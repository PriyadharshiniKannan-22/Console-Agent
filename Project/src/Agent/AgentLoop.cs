using System.Text;
using System.Text.Json.Nodes;

namespace ToolForge.Agent;

public class AgentLoop
{
    private readonly HttpClient _http;
    private readonly ToolRegistry _tools;
    private readonly string _apiKey;
    private readonly string _model = "meta-llama/llama-4-scout-17b-16e-instruct";

    public AgentLoop(ToolRegistry tools)
    {
        _tools = tools;

        _apiKey =
            Environment.GetEnvironmentVariable("GROQ_API_KEY")
            ?? throw new InvalidOperationException(
                "GROQ_API_KEY environment variable not found."
            );

        _http = new HttpClient();

        _http.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {_apiKey}"
        );
    }

    public async Task<AgentResult> RunAsync(string userInput)
    {
        var messages = new JsonArray
        {
            new JsonObject
            {
                ["role"] = "system",
                ["content"] = PromptBuilder.SystemPrompt()
            },
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = userInput
            }
        };

        int totalInputTokens = 0;
        int totalOutputTokens = 0;
        int toolCalls = 0;

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"LOOP ITERATION {i}");

            var requestBody = new JsonObject
            {
                ["model"] = _model,
                ["messages"] = JsonNode.Parse(messages.ToJsonString()),
                ["tools"] = JsonNode.Parse(
                    _tools.GetToolDefinitions().ToJsonString()
                ),
                ["tool_choice"] = "auto"
            };

            Console.WriteLine("SENDING REQUEST TO GROQ");

            Console.WriteLine("===== REQUEST =====");
            Console.WriteLine(requestBody.ToJsonString());
            Console.WriteLine("===================");

            var response = await _http.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                new StringContent(
                    requestBody.ToJsonString(),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            var responseBody =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine();
            Console.WriteLine("========== GROQ JSON ==========");
            Console.WriteLine(responseBody);
            Console.WriteLine("================================");
            Console.WriteLine();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Groq Error ({response.StatusCode}):\n{responseBody}"
                );
            }

            var json = JsonNode.Parse(responseBody)!;

            var usage = json["usage"];

            if (usage != null)
            {
                totalInputTokens +=
                    usage["prompt_tokens"]?.GetValue<int>() ?? 0;

                totalOutputTokens +=
                    usage["completion_tokens"]?.GetValue<int>() ?? 0;
            }

            var choice =
                json["choices"]![0]!;

            var message =
                choice["message"]!;

            var finishReason =
                choice["finish_reason"]!
                    .GetValue<string>();

            Console.WriteLine(
                $"Finish Reason: {finishReason}"
            );

            messages.Add(
                JsonNode.Parse(
                    message.ToJsonString()
                )!
            );

            if (finishReason == "stop")
            {
                return new AgentResult
                {
                    Reply =
                        message["content"]?.GetValue<string>()
                        ?? "No response.",

                    InputTokens = totalInputTokens,
                    OutputTokens = totalOutputTokens,
                    ToolCalls = toolCalls
                };
            }

            if (finishReason == "tool_calls")
            {
                toolCalls++;

                var toolCallsArray =
                    message["tool_calls"]!.AsArray();

                foreach (var toolCall in toolCallsArray)
                {
                    var toolName =
                        toolCall!["function"]!["name"]!
                            .GetValue<string>();

                    var toolArgs =
                        toolCall["function"]!["arguments"]!
                            .GetValue<string>();

                    var toolCallId =
                        toolCall["id"]!
                            .GetValue<string>();

                    Console.ForegroundColor =
                        ConsoleColor.Yellow;

                    Console.WriteLine(
                        $"[TOOL] {toolName}"
                    );

                    Console.WriteLine(toolArgs);

                    Console.ResetColor();

                    var result =
                        await _tools.InvokeAsync(
                            toolName,
                            toolArgs
                        );

                    Console.WriteLine("ADDING TOOL MESSAGE");

                    Console.ForegroundColor =
                        ConsoleColor.Cyan;

                    Console.WriteLine(
                        $"[RESULT] {result}"
                    );

                    Console.ResetColor();

                    messages.Add(
                        new JsonObject
                        {
                            ["role"] = "tool",
                            ["tool_call_id"] = toolCallId,
                            ["name"] = toolName,
                            ["content"] = result
                        }
                    );

                    messages.Add(
                        new JsonObject
                        {
                            ["role"] = "user",
                            ["content"] =
                                "The tool has returned its result. " +
                                "Answer the user's original question using ONLY the tool result."
                        }
                    );

                    Console.WriteLine();
                    Console.WriteLine("===== MESSAGES AFTER TOOL =====");
                    Console.WriteLine(messages.ToJsonString());
                    Console.WriteLine("===============================");
                    Console.WriteLine();
                }
                
                Console.WriteLine("CONTINUING TO NEXT ITERATION");
                
                continue;
            }

            return new AgentResult
            {
                Reply = "Unexpected finish reason.",
                InputTokens = totalInputTokens,
                OutputTokens = totalOutputTokens,
                ToolCalls = toolCalls
            };
        }

        return new AgentResult
        {
            Reply = "Agent stopped after max iterations.",
            InputTokens = totalInputTokens,
            OutputTokens = totalOutputTokens,
            ToolCalls = toolCalls
        };
    }
}