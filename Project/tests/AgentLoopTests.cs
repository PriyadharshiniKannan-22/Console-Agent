using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ToolForge.Agent;
using Xunit;

namespace ToolForge.Tests;

/// <summary>
/// Unit tests for AgentLoop behaviour:
///   - correct tool routing
///   - loop terminates on finish_reason == "stop"
///   - max-iteration guard fires
///   - empty / whitespace input handled
///
/// NOTE: These tests use a FakeToolRegistry and a stubbed HttpClient so
/// no real API keys are needed. All HTTP responses are hand-crafted JSON
/// matching the OpenAI Chat Completions shape.
/// </summary>
public class AgentLoopTests
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a minimal OpenAI-compatible response JSON.
    /// </summary>
    private static string StopResponse(string content) => $$"""
        {
          "choices": [{
            "message": { "role": "assistant", "content": "{{content}}" },
            "finish_reason": "stop"
          }]
        }
        """;

    /// <summary>
    /// Builds a tool_call response that requests one tool invocation.
    /// </summary>
    private static string ToolCallResponse(string toolName, string argsJson) => $$"""
        {
          "choices": [{
            "message": {
              "role": "assistant",
              "content": null,
              "tool_calls": [{
                "id": "call_001",
                "type": "function",
                "function": {
                  "name": "{{toolName}}",
                  "arguments": "{{argsJson.Replace("\"", "\\\"")}}"
                }
              }]
            },
            "finish_reason": "tool_calls"
          }]
        }
        """;

    /// <summary>
    /// Creates an AgentLoop wired to a fake HTTP handler that returns
    /// the given sequence of responses in order.
    /// </summary>
    private static AgentLoop BuildAgent(
        FakeToolRegistry tools,
        params string[] httpResponses)
    {
        var queue = new Queue<string>(httpResponses);
        var handler = new FakeHttpHandler(() =>
            queue.TryDequeue(out var r) ? r : StopResponse("done"));
        var client = new HttpClient(handler);
        return new AgentLoop(tools, client);
    }

    // ── Tests ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Loop_StopsImmediately_WhenFinishReasonIsStop()
    {
        var tools = new FakeToolRegistry();
        var agent = BuildAgent(tools, StopResponse("Hello from ToolForge!"));

        var result = await agent.RunAsync("Hi there", "anthropic", "test-model");

        Assert.Equal("Hello from ToolForge!", result.Reply);
        Assert.Equal(0, result.ToolCallCount);
    }

    [Fact]
    public async Task Loop_InvokesTool_ThenReturnsAnswer()
    {
        var tools = new FakeToolRegistry();
        tools.Register("get_time", _ => "2026-06-09 10:30:00 IST");

        var agent = BuildAgent(tools,
            ToolCallResponse("get_time", """{"timezone":"Asia/Kolkata"}"""),
            StopResponse("The time in India is 10:30 AM IST.")
        );

        var result = await agent.RunAsync("What time is it in India?", "openai", "gpt-4o-mini");

        Assert.Contains("10:30", result.Reply);
        Assert.Equal(1, result.ToolCallCount);
        Assert.True(tools.WasCalled("get_time"));
    }

    [Fact]
    public async Task Loop_CanChainMultipleTools()
    {
        var tools = new FakeToolRegistry();
        tools.Register("get_weather", _ => "Temp: 34°C, Partly Cloudy");
        tools.Register("get_time",    _ => "2026-06-09 14:00:00 IST");

        var agent = BuildAgent(tools,
            ToolCallResponse("get_weather", """{"city":"Chennai"}"""),
            ToolCallResponse("get_time",    """{"timezone":"Asia/Kolkata"}"""),
            StopResponse("Chennai: 34°C partly cloudy at 2:00 PM IST.")
        );

        var result = await agent.RunAsync(
            "What is the weather and time in Chennai?", "anthropic", "claude-3-5-haiku-20241022");

        Assert.Equal(2, result.ToolCallCount);
        Assert.True(tools.WasCalled("get_weather"));
        Assert.True(tools.WasCalled("get_time"));
    }

    [Fact]
    public async Task Loop_RespectsMaxIterationLimit()
    {
        var tools = new FakeToolRegistry();
        tools.Register("get_weather", _ => "keep going");

        // Return tool_calls forever — the loop must self-terminate
        var infiniteToolCall = ToolCallResponse("get_weather", """{"city":"Anywhere"}""");
        var agent = BuildAgent(tools,
            infiniteToolCall, infiniteToolCall, infiniteToolCall,
            infiniteToolCall, infiniteToolCall, infiniteToolCall); // 6 = over the 5 limit

        var result = await agent.RunAsync("Weather?", "openai", "gpt-4o-mini");

        Assert.Contains("Max iterations", result.Reply);
    }

    [Fact]
    public async Task Loop_HandlesToolError_Gracefully()
    {
        var tools = new FakeToolRegistry();
        // Tool returns an error string (not an exception)
        tools.Register("get_weather", _ => "City 'Xyz123' not found.");

        var agent = BuildAgent(tools,
            ToolCallResponse("get_weather", """{"city":"Xyz123"}"""),
            StopResponse("I couldn't find weather data for Xyz123. Try a real city name.")
        );

        var result = await agent.RunAsync("Weather in Xyz123?", "openai", "gpt-4o-mini");

        Assert.Equal(1, result.ToolCallCount);
        Assert.DoesNotContain("Exception", result.Reply);
    }

    [Fact]
    public async Task PromptBuilder_SystemPrompt_ContainsAllTools()
    {
        var prompt = PromptBuilder.SystemPrompt();
        Assert.Contains("get_weather", prompt);
        Assert.Contains("get_time",    prompt);
        Assert.Contains("query_sqlite", prompt);
    }

    [Fact]
    public async Task PromptBuilder_WrapUserMessage_IncludesProvider()
    {
        var wrapped = PromptBuilder.WrapUserMessage("Hello", "anthropic", "claude-3-5-haiku-20241022");
        Assert.Contains("anthropic", wrapped);
        Assert.Contains("Hello", wrapped);
    }
}

// ── Test Doubles ─────────────────────────────────────────────────────────────

/// <summary>Fake ToolRegistry that maps tool names to delegate implementations.</summary>
public class FakeToolRegistry : ToolRegistry
{
    private readonly Dictionary<string, Func<string, string>> _handlers = new();
    private readonly HashSet<string> _called = new();

    public FakeToolRegistry() : base(":memory:") { }

    public void Register(string toolName, Func<string, string> handler)
        => _handlers[toolName] = handler;

    public bool WasCalled(string toolName) => _called.Contains(toolName);

    public override Task<string> InvokeAsync(string toolName, string argsJson)
    {
        _called.Add(toolName);
        return Task.FromResult(
            _handlers.TryGetValue(toolName, out var h)
                ? h(argsJson)
                : $"No fake registered for '{toolName}'");
    }
}

/// <summary>Fake HttpMessageHandler that returns queued response strings.</summary>
public class FakeHttpHandler : HttpMessageHandler
{
    private readonly Func<string> _nextResponse;

    public FakeHttpHandler(Func<string> nextResponse)
        => _nextResponse = nextResponse;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var body    = _nextResponse();
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
    }
}
