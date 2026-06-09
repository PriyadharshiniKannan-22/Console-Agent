using System.Text.Json;
using System.Text.Json.Nodes;
using ToolForge.Tools;

namespace ToolForge.Agent;

public class ToolRegistry
{
    private readonly WeatherTool _weather = new();
    private readonly TimeTool _time = new();
    private readonly SqliteTool _sqlite;

    public ToolRegistry(string dbPath)
    {
        _sqlite = new SqliteTool(dbPath);
    }

    public JsonArray GetToolDefinitions()
    {
        return new JsonArray
        {
            JsonNode.Parse("""
            {
              "type": "function",
              "function": {
                "name": "get_weather",
                "description": "Get current weather for a city.",
                "parameters": {
                  "type": "object",
                  "properties": {
                    "city": {
                      "type": "string"
                    }
                  },
                  "required": ["city"]
                }
              }
            }
            """)!,

            JsonNode.Parse("""
            {
              "type": "function",
              "function": {
                "name": "get_time",
                "description": "Get current time in a timezone.",
                "parameters": {
                  "type": "object",
                  "properties": {
                    "timezone": {
                      "type": "string"
                    }
                  },
                  "required": ["timezone"]
                }
              }
            }
            """)!,

            JsonNode.Parse("""
            {
              "type": "function",
              "function": {
                "name": "query_sqlite",
                "description": "Run a SELECT query on the SQLite database.",
                "parameters": {
                  "type": "object",
                  "properties": {
                    "query": {
                      "type": "string"
                    }
                  },
                  "required": ["query"]
                }
              }
            }
            """)!
        };
    }

    public async Task<string> InvokeAsync(string toolName, string argsJson)
    {
        var args = JsonSerializer.Deserialize<Dictionary<string, string>>(argsJson)
                   ?? new Dictionary<string, string>();

        return toolName switch
        {
            "get_weather" =>
                await _weather.GetWeatherAsync(
                    args.GetValueOrDefault("city", "")
                ),

            "get_time" =>
                _time.GetTime(
                    args.GetValueOrDefault("timezone", "UTC")
                ),

            "query_sqlite" =>
                await _sqlite.QueryAsync(
                    args.GetValueOrDefault("query", "")
                ),

            _ => $"Unknown tool: {toolName}"
        };
    }
}