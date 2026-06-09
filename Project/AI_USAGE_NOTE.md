# AI Usage Note — ToolForge Agent
**Team:** ToolForge | **Submission Date:** June 2026

---

## What AI Helped With

| Area | How AI Was Used |
|------|----------------|
| Agent Loop Design | Scaffolded the ReAct (Reason+Act) loop pattern in C# |
| Function Calling Schema | Generated OpenAI tool definition JSON for all 3 tools |
| SQLite Integration | Wrote parameterized query executor with injection protection |
| Error Handling | Suggested try/catch patterns for HTTP timeouts and API errors |
| Unit Tests | Generated xUnit test scaffolding for happy path + edge cases |
| README | Drafted architecture overview and setup instructions |

---

## What AI Got Wrong

| Issue | What Happened | How We Fixed It |
|-------|--------------|----------------|
| Async deadlocks | AI used `.Result` on async calls (classic C# mistake) | Changed to `await` throughout |
| Tool loop termination | Agent loop didn't stop when `finish_reason == "stop"` | Added explicit loop exit condition |
| SQL injection guard | First version allowed `DROP TABLE` — AI missed this | Added allowlist: only `SELECT` permitted |
| Timezone parsing | `get_time()` used incorrect `TimeZoneInfo` ID format on Linux | Used `TimeZoneConverter` NuGet package |

---

## Best Prompts Used

### 1. Agent Loop Bootstrap
```
You are a C# expert. Build a console AI agent loop that:
1. Sends user input + tool definitions to OpenAI Chat Completions API
2. If the response contains tool_calls, execute the matching C# method
3. Append the tool result as a "tool" role message
4. Loop until finish_reason is "stop"
Use async/await throughout. Model: gpt-4o-mini. No external agent frameworks.
```

### 2. Tool Safety Validation
```
Write a C# method that validates a raw SQL string before execution on SQLite.
Allow only SELECT statements. Reject any string containing: DROP, DELETE, INSERT,
UPDATE, ALTER, CREATE, TRUNCATE (case-insensitive). Throw InvalidOperationException
with a clear message if blocked.
```

### 3. OpenAI Function Schema Generator
```
Generate the OpenAI function-calling JSON tool definition for a C# method:
  string GetWeather(string city)
The tool should clearly describe what it does, its parameter name, type, and
that it is required. Output raw JSON only.
```

### 4. xUnit Test Scaffolding
```
Write xUnit tests for a C# class WeatherTool with method GetWeather(string city).
Cover: valid city returns non-null string, empty city throws ArgumentException,
invalid city returns a user-friendly error string (not an exception).
Mock HttpClient using Moq.
```

---

## Overall AI Assistance Rating

- **Speed boost:** ~60% faster than writing from scratch
- **Quality:** Good for boilerplate; required human review for logic and safety
- **Most valuable:** Generating JSON schemas and async scaffolding
- **Least valuable:** Business logic and security — always verify AI output
