# 🤖 ToolForge Agent
### A Tool-Calling AI Agent in C# — Console Prototype

> *"Bridging the gap between AI reasoning and real-world execution — in C#, not just Python."*

---

## 🧠 What Is This?

**ToolForge Agent** is a console-based AI agent built entirely in C# that demonstrates **tool-calling / function-calling** via the OpenAI (or Anthropic) API.

Most agent frameworks live in Python. This project proves that **C# is a first-class citizen** for building production-grade AI agent loops — with clean architecture, strong typing, and enterprise-grade patterns.

The user types a natural language question. The agent reasons, decides which tool(s) to invoke, calls them, and returns a grounded answer.

---

## 🏗️ Architecture Overview

```
User Input
    │
    ▼
┌─────────────────────────────────┐
│         Console UI Layer        │  ← Member 4
│   (Input / Output / Formatting) │
└────────────────┬────────────────┘
                 │
                 ▼
┌─────────────────────────────────┐
│        AI Agent Loop            │  ← Member 1
│  (OpenAI Function Calling API)  │
│  [Reason → Tool Call → Observe] │
└────────┬──────────┬─────────────┘
         │          │
    ┌────▼───┐  ┌───▼──────────────────┐
    │ Tools  │  │   SQLite Query Tool   │
    │ Layer  │  │   (Structured Data)   │
    │        │  └──────────────────────┘
    │ • get_weather()   ← Member 2      Member 3 ──► SQLite Schema
    │ • get_time()                                   Sample Data
    │ • query_sqlite()                               SQL Executor
    └────────┘
```

### Agent Loop (ReAct Pattern)
```
[Think] → Does this need a tool?
[Act]   → Call the appropriate tool with parameters
[Observe] → Receive tool result
[Think] → Is the answer complete?
[Respond] → Return final answer to user
```

---

## 👥 Team: **ToolForge**

> *We forge tools that AI agents wield.*

| Member | Role | Responsibility |
|--------|------|----------------|
| Member 1 | AI Agent Architect | Agent loop, OpenAI function calling, prompt engineering |
| Member 2 | Backend & Tool Developer | `get_weather()`, `get_time()`, error handling |
| Member 3 | Data & Database Engineer | SQLite schema, sample data, `query_sqlite()` |
| Member 4 | Integration & Demo Lead | Console UI, testing, final integration, demo |

---

## 🛠️ Technology Stack

| Category | Technology |
|----------|-----------|
| Language | C# (.NET 8) |
| AI API | OpenAI API (Free Tier / GPT-4o-mini) |
| Weather API | OpenWeatherMap (Free Tier) |
| Database | SQLite (via `Microsoft.Data.Sqlite`) |
| Testing | xUnit |
| Source Control | GitHub |
| Tooling | `dotnet` CLI — 100% Free/Open-Source |

---

## ⚙️ Setup Instructions

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (free)
- [OpenAI API Key](https://platform.openai.com/) (free tier works)
- [OpenWeatherMap API Key](https://openweathermap.org/api) (free tier)

### 1. Clone the Repository
```bash
git clone https://github.com/<your-org>/toolforge-agent.git
cd toolforge-agent
```

### 2. Set Environment Variables
```bash
# Windows (PowerShell)
$env:OPENAI_API_KEY = "sk-..."
$env:OPENWEATHER_API_KEY = "your-key-here"

# Linux / macOS
export OPENAI_API_KEY="sk-..."
export OPENWEATHER_API_KEY="your-key-here"
```

### 3. Restore Dependencies
```bash
cd src
dotnet restore
```

### 4. Seed the Database
```bash
dotnet run --project Tools -- --seed
```

### 5. Run the Agent
```bash
dotnet run --project ConsoleAgent
```

---

## 🚀 Run Instructions

Once running, type any natural language query:

```
🤖 ToolForge Agent Ready. Type your question (or 'exit' to quit):

> What is the weather in Chennai right now?
[Agent] Invoking tool: get_weather({ "city": "Chennai" })
[Tool]  Temp: 34°C, Humidity: 72%, Condition: Partly Cloudy
[Agent] The current weather in Chennai is 34°C and partly cloudy with 72% humidity.

> What time is it in Tokyo?
[Agent] Invoking tool: get_time({ "timezone": "Asia/Tokyo" })
[Tool]  2026-06-08 17:45:03 JST
[Agent] It is currently 5:45 PM in Tokyo (JST).

> Who are the top 3 employees by sales this quarter?
[Agent] Invoking tool: query_sqlite({ "query": "SELECT name, sales FROM employees ORDER BY sales DESC LIMIT 3" })
[Tool]  [{"name":"Priya","sales":142000},{"name":"Arjun","sales":138500},...]
[Agent] The top 3 employees by sales this quarter are Priya (₹1,42,000), Arjun (₹1,38,500)...
```

---

## 📁 Repository Structure

```
toolforge-agent/
├── src/
│   ├── Agent/
│   │   ├── AgentLoop.cs          # Core ReAct loop + function calling
│   │   ├── ToolRegistry.cs       # Tool registration & dispatch
│   │   └── PromptBuilder.cs      # System prompt construction
│   ├── Tools/
│   │   ├── WeatherTool.cs        # get_weather() — OpenWeatherMap
│   │   ├── TimeTool.cs           # get_time() — timezone-aware
│   │   └── SqliteTool.cs         # query_sqlite() — safe SQL executor
│   ├── Database/
│   │   ├── SchemaSetup.cs        # DB init & migration
│   │   ├── SeedData.cs           # Sample dataset loader
│   │   └── QueryExecutor.cs      # Parameterized query runner
│   ├── UI/
│   │   └── ConsoleRenderer.cs    # Formatted console I/O
│   └── Program.cs                # Entry point
├── tests/
│   ├── AgentLoopTests.cs
│   ├── WeatherToolTests.cs
│   ├── TimeToolTests.cs
│   └── SqliteToolTests.cs
├── sample-data/
│   ├── employees.csv
│   ├── products.json
│   └── expected-outputs.md
├── docs/
│   └── architecture-diagram.png
├── prompts/
│   └── ai-usage-note.md          # Prompt log & AI usage
├── .env.example
├── README.md
└── ConsoleAgent.sln
```

---

## 🧪 Running Tests

```bash
cd tests
dotnet test --verbosity normal
```

Tests cover:
- `get_time()` returns correct timezone-aware datetime
- `get_weather()` handles invalid city gracefully
- `query_sqlite()` rejects destructive SQL (DROP, DELETE, etc.)
- Agent loop correctly routes multi-tool queries

---

## 🔒 Assumptions & Limitations

| Area | Detail |
|------|--------|
| API Keys | Must be set as environment variables; never committed to repo |
| SQL Safety | Only `SELECT` queries permitted; write operations blocked |
| Free Tier Limits | OpenAI free tier has rate limits; add retry logic for production |
| Model | Uses `gpt-4o-mini` by default (cheapest, fast, supports function calling) |
| Concurrency | Single-threaded console loop; not designed for concurrent sessions |
| Weather | Results depend on OpenWeatherMap availability |

---

## 📝 AI Usage Note

See [`prompts/ai-usage-note.md`](./prompts/ai-usage-note.md) for:
- What AI helped build
- Where AI went wrong
- Best prompts used during development

---

## 📹 Demo Video

[▶ Watch on Loom](#) *(link added after recording)*

---

## 📄 License

MIT — Free to use, modify, and distribute.
