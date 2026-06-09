# 🔧 ToolForge Agent
### A Tool-Calling AI Agent in C# — Console + Web Prototype

> *"Bridging the gap between AI reasoning and real-world execution — in C#, not just Python."*

---

## 🧠 What Is This?

**ToolForge Agent** is a console-based AI agent built entirely in **C# (.NET 8)** that demonstrates production-grade **tool-calling / function-calling** via the OpenAI or Anthropic API.

Most agent frameworks are Python-only. This project proves that **C# is a first-class citizen** for building AI agent loops — with clean architecture, strong typing, and enterprise-grade patterns that translate directly to real-world .NET applications.

The user types a natural language question. The agent **reasons**, decides which tool(s) to invoke, calls them with structured arguments, receives the result, and returns a grounded final answer — all in a live ReAct loop.

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🔁 **ReAct Agent Loop** | Think → Act → Observe cycle drives every response |
| 🌦️ **Live Weather Tool** | `get_weather()` fetches real-time data via OpenWeatherMap API |
| 🕐 **Timezone-Aware Time Tool** | `get_time()` supports any IANA timezone (Windows + Linux) |
| 🗄️ **SQLite Query Tool** | `query_sqlite()` runs safe SELECT queries on a local database |
| 🔀 **Provider Switching** | Toggle between **Anthropic** and **OpenAI** at runtime |
| 📊 **Token Stats Dashboard** | Tracks input tokens, output tokens, tool calls, and estimated cost |
| 🛡️ **SQL Injection Guard** | Blocks DROP, DELETE, INSERT, UPDATE, and all destructive SQL |
| 🌐 **Web UI** | Terminal-style chat interface served by ASP.NET Core |
| ⛓️ **Multi-Tool Chaining** | Agent can invoke multiple tools in sequence for compound queries |
| 🔄 **Fallback Handling** | Graceful error messages when tools fail — agent never crashes |

---

## 🏗️ Architecture Overview

```
User Input (Console / Web UI)
         │
         ▼
┌─────────────────────────────────────┐
│          Console UI Layer           │  ← Member 4
│      ConsoleRenderer.cs             │
│   (Prompt, Banner, Colour Output)   │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│         AI Agent Loop               │  ← Member 1
│  AgentLoop.cs + PromptBuilder.cs    │
│                                     │
│  [Think] Is a tool needed?          │
│  [Act]   Call tool with args JSON   │
│  [Obs]   Receive tool result        │
│  [Think] Is the answer complete?    │
│  [Reply] Return grounded answer     │
└──────┬──────────────────┬───────────┘
       │                  │
       ▼                  ▼
┌──────────────┐  ┌────────────────────┐
│  Tool Layer  │  │  SQLite Tool       │
│  Member 2    │  │  Member 3          │
│              │  │                    │
│ get_weather()│  │ query_sqlite()     │
│ get_time()   │  │ SchemaSetup.cs     │
└──────┬───────┘  │ SeedData.cs        │
       │          └────────┬───────────┘
       ▼                   ▼
 OpenWeatherMap API    toolforge.db (SQLite)
```

### ReAct Loop — Step by Step
```
[Think]    → "Does this question need real-time data or a database lookup?"
[Act]      → Invoke get_weather / get_time / query_sqlite with structured args
[Observe]  → Receive the tool result as a string
[Think]    → "Is the result sufficient to answer the user?"
[Respond]  → Synthesise a natural language final answer
```

---

## 👥 Team: **ToolForge**

> *"We forge the tools that AI agents wield."*

| Member | Role | Core Responsibility | Presentation Line |
|--------|------|---------------------|-------------------|
| **Member 1** | AI Agent Architect | `AgentLoop.cs`, `PromptBuilder.cs`, OpenAI/Anthropic function calling, prompt engineering | *"I designed the agent workflow that enables the model to reason about when and which tool should be invoked."* |
| **Member 2** | Backend & Tool Developer | `WeatherTool.cs`, `TimeTool.cs`, HTTP clients, error handling | *"I implemented the tools that the AI agent can invoke to interact with external systems."* |
| **Member 3** | Data & Database Engineer | `SchemaSetup.cs`, `SeedData.cs`, `SqliteTool.cs`, SQL guard, sample dataset | *"I designed the structured knowledge layer that allows the AI agent to retrieve factual information from a database."* |
| **Member 4** | Integration & Demo Lead | `ConsoleRenderer.cs`, `ChatController.cs`, xUnit tests, final integration, demo | *"I handled the user interaction layer and integrated all components into a seamless working prototype."* |

---

## 🛠️ Technology Stack

| Category | Technology | Purpose |
|----------|-----------|---------|
| Language | C# (.NET 8) | Core application |
| AI API (Primary) | Anthropic Claude API | `claude-3-5-haiku-20241022` — function calling |
| AI API (Secondary) | OpenAI API | `gpt-4o-mini` — function calling |
| Weather API | OpenWeatherMap (Free Tier) | `get_weather()` tool |
| Database | SQLite via `Microsoft.Data.Sqlite` | `query_sqlite()` tool |
| Web Framework | ASP.NET Core (built-in) | Serves web UI + `/api/chat` endpoint |
| Testing | xUnit + `Microsoft.NET.Test.Sdk` | Unit & integration tests |
| Timezones | `TimeZoneConverter` NuGet | Cross-platform IANA timezone support |
| Source Control | GitHub | Version control |
| Tooling | `dotnet` CLI | 100% free / open-source |

---

## 📁 Folder Structure

```
toolforge-agent/
│
├── src/                          # All production C# source code
│   ├── Agent/
│   │   ├── AgentLoop.cs          # ReAct loop — sends messages, handles tool_calls
│   │   ├── ToolRegistry.cs       # Registers tools, dispatches invocations
│   │   └── PromptBuilder.cs      # System prompt, user message wrappers, nudge prompts
│   ├── Tools/
│   │   ├── WeatherTool.cs        # get_weather() — OpenWeatherMap REST call
│   │   ├── TimeTool.cs           # get_time() — IANA timezone-aware
│   │   └── SqliteTool.cs         # query_sqlite() — SELECT-only SQL guard
│   ├── Database/
│   │   ├── SchemaSetup.cs        # Creates tables on first run
│   │   └── SeedData.cs           # Seeds employees + products sample data
│   ├── UI/
│   │   └── ConsoleRenderer.cs    # Banner, colour-coded prompts, formatted output
│   └── Program.cs                # Entry point — bootstrap, DB init, main loop
│
├── api/                          # Thin HTTP layer (web UI ↔ AgentLoop)
│   ├── ChatController.cs         # POST /api/chat → AgentLoop.RunAsync()
│   ├── Startup.cs                # DI, CORS, static file serving
│   └── Models/
│       └── ChatModels.cs         # ChatRequest, ChatResponse, UsageStats
│
├── web/                          # Frontend — served as static files
│   ├── index.html                # Terminal-style chat UI (dark mode)
│   └── js/
│       ├── agent.js              # callAgentAPI() — wires UI to POST /api/chat
│       └── tokenStats.js         # calcCost(), fmtCost() — live token pricing
│
├── tests/                        # xUnit test project
│   ├── AgentLoopTests.cs         # ReAct loop routing, termination, chaining
│   ├── AgentToolTests.cs         # WeatherTool, TimeTool happy paths
│   └── SqliteToolTests.cs        # SQL guard, seed queries, edge cases
│
├── sample-data/
│   ├── employees.csv             # Raw seed data
│   ├── products.json             # Raw seed data
│   └── expected-outputs.md       # Query → expected tool call → expected answer
│
├── prompts/
│   └── ai-usage-note.md          # What AI helped with, what it got wrong, best prompts
│
├── docs/
│   └── architecture.md           # Extended architecture notes
│
├── .env.example                  # Template — copy to .env and fill keys
├── .gitignore                    # Excludes .env, bin/, obj/, *.db
├── AgentEngine.csproj            # Project file with all NuGet references
├── LICENSE                       # MIT
└── README.md                     # This file
```

---

## ⚙️ Installation & Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — verify with `dotnet --version`
- [Anthropic API Key](https://console.anthropic.com) — ~$5 free credits on signup, no credit card needed
- [OpenAI API Key](https://platform.openai.com) — ~$5 free credits on signup
- [OpenWeatherMap API Key](https://openweathermap.org) — permanently free tier, 60 calls/min

### Step 1 — Clone the Repository

```bash
git clone https://github.com/PriyadharshiniKannan-22/Console-Agent
cd Project
```

### Step 2 — Configure API Keys

Copy the example env file and fill in your keys:

```bash
cp .env.example .env
```

Edit `.env`:

```env
ANTHROPIC_API_KEY=sk-ant-api03-xxxxxxxxxxxxxxxxxxxx
OPENAI_API_KEY=sk-proj-xxxxxxxxxxxxxxxxxxxx
OPENWEATHER_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

Or set them directly in your terminal:

```bash
# Windows (PowerShell)
$env:ANTHROPIC_API_KEY   = "sk-ant-api03-..."
$env:OPENAI_API_KEY      = "sk-proj-..."
$env:OPENWEATHER_API_KEY = "your-key-here"

# macOS / Linux
export ANTHROPIC_API_KEY="sk-ant-api03-..."
export OPENAI_API_KEY="sk-proj-..."
export OPENWEATHER_API_KEY="your-key-here"
```

> ⚠️ **Never commit `.env` to GitHub.** The `.gitignore` already excludes it — verify before your first push.

### Step 3 — Install NuGet Packages

```bash
cd src
dotnet add package Microsoft.Data.Sqlite
dotnet add package TimeZoneConverter

cd ../tests
dotnet add package Microsoft.Data.Sqlite
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
```

### Step 4 — Build the Project

```bash
dotnet restore
dotnet build
```

### Step 5 — Run the Agent

```bash
dotnet run --project src
```

On first run, the agent will:
1. Create `toolforge.db` in the output directory
2. Auto-create `employees` and `products` tables
3. Seed 7 employees and 7 products as sample data
4. Start the console loop and print the banner

---

## 🚀 Usage

Once running, type any natural language question and press Enter:

```
  ████████╗ ██████╗  ██████╗ ██╗     ███████╗ ██████╗ ██████╗  ██████╗ ███████╗
  ...
  Tool-Calling AI Agent in C# | Team ToolForge
  Tools: get_weather | get_time | query_sqlite

  Try: 'What's the weather in Mumbai?'
       'What time is it in London?'
       'Show me the top 3 employees by sales'
       'List all electronics under 5000'
  Type 'exit' to quit.

🤖 You > What is the weather in Chennai right now?
  [Agent] Invoking tool: get_weather({"city":"Chennai"})
  [Tool]  City: Chennai | Temp: 34.2°C | Humidity: 72% | Condition: partly cloudy

💡 Agent > The current weather in Chennai is 34.2°C with 72% humidity. Conditions are partly cloudy — a typical summer day.

🤖 You > What time is it in Tokyo right now?
  [Agent] Invoking tool: get_time({"timezone":"Asia/Tokyo"})
  [Tool]  2026-06-09 17:45:03 JST

💡 Agent > It is currently 5:45 PM in Tokyo (Japan Standard Time).

🤖 You > Who are the top 3 employees by sales?
  [Agent] Invoking tool: query_sqlite({"query":"SELECT name, sales FROM employees ORDER BY sales DESC LIMIT 3"})
  [Tool]  [{"name":"Priya Sharma","sales":142000},{"name":"Arjun Nair","sales":138500},{"name":"Meera Iyer","sales":125000}]

💡 Agent > The top 3 employees by sales are:
           1. Priya Sharma — ₹1,42,000
           2. Arjun Nair   — ₹1,38,500
           3. Meera Iyer   — ₹1,25,000
```

---

## 🔀 Provider & Model Switching

Switch providers at runtime via the web UI toggle or by updating the request:

| Provider | Model String | Best For |
|----------|-------------|---------|
| Anthropic | `claude-3-5-haiku-20241022` | Default — fast, cheap, full tool calling |
| Anthropic | `claude-sonnet-4-6` | Higher reasoning quality |
| OpenAI | `gpt-4o-mini` | Lowest cost, full function calling |
| OpenAI | `gpt-4o` | Higher accuracy on complex queries |

---

## 📊 Token Stats

The web UI tracks per-session usage:

| Metric | Description |
|--------|-------------|
| Input Tokens | Tokens sent to the model (prompt + tool results) |
| Output Tokens | Tokens generated by the model |
| Tool Calls | Number of tool invocations in the session |
| Messages | Total turns (user + assistant) |
| Est. Cost | Calculated from live pricing tables in `tokenStats.js` |

---

## 🗄️ Sample Data

### Employees Table

| id | name | dept | role | sales | joined |
|----|------|------|------|-------|--------|
| 1 | Priya Sharma | Sales | Senior Rep | 142000 | 2022-03-15 |
| 2 | Arjun Nair | Sales | Rep | 138500 | 2023-01-10 |
| 3 | Meera Iyer | Sales | Rep | 125000 | 2022-09-01 |
| 4 | Vikram Rao | Engineering | Engineer | 0 | 2021-06-20 |
| 5 | Lakshmi Kumar | HR | Manager | 0 | 2020-11-05 |

### Products Table

| id | name | category | price | stock |
|----|------|---------|-------|-------|
| 1 | Laptop Pro 15 | Electronics | 85000 | 42 |
| 2 | Wireless Mouse | Electronics | 1200 | 150 |
| 3 | Office Chair | Furniture | 18000 | 28 |
| 4 | Standing Desk | Furniture | 32000 | 12 |

### Sample Queries to Try

```
Show me all products in the Furniture category
Which employees joined after 2022?
What is the average sales across all Sales department employees?
List all electronics products under ₹5000 sorted by price
```

---

## 🧪 Running Tests

```bash
cd tests
dotnet test --verbosity normal
```

### Test Coverage

| Test File | What It Tests |
|-----------|--------------|
| `AgentLoopTests.cs` | Loop terminates on `finish_reason=stop`, routes tool calls correctly, max-iteration guard, multi-tool chaining |
| `AgentToolTests.cs` | `get_weather()` empty city throws, `get_time()` valid/invalid timezone, happy paths |
| `SqliteToolTests.cs` | Valid SELECT returns JSON, destructive SQL blocked (DROP, DELETE, INSERT, UPDATE, ALTER, TRUNCATE), empty query, syntax errors |

All agent loop tests run **offline** using a `FakeHttpHandler` — no API keys required to run the test suite.

---

## 🔒 Assumptions & Limitations

| Area | Detail |
|------|--------|
| API Keys | Must be set as environment variables; never hardcoded or committed |
| SQL Safety | Only `SELECT` statements permitted; all write operations are blocked at string level |
| Free Tier Limits | Both Anthropic and OpenAI free tiers have RPM limits; add exponential backoff for production |
| Model | Uses `claude-3-5-haiku-20241022` by default — cheapest Anthropic model with full tool calling |
| Concurrency | Single-threaded console loop; the ASP.NET layer handles one request at a time |
| Weather Accuracy | Depends on OpenWeatherMap uptime and data freshness |
| New OWM Keys | OpenWeatherMap API keys take up to 2 hours to activate after signup |
| SQLite | Single-file local database; not designed for multi-user concurrent writes |

---

## 📝 AI Usage Note

See [`prompts/ai-usage-note.md`](./prompts/ai-usage-note.md) for the full breakdown:

- ✅ What AI helped build (agent loop scaffolding, JSON schemas, async patterns, test boilerplate)
- ❌ What AI got wrong (async deadlocks with `.Result`, missing loop exit conditions, SQL injection gaps)
- 💡 Best prompts used during development

---

## 📹 Demo Video

[▶ Watch on Loom](#) *(link added after recording)*

**Demo covers:**
- Startup sequence and DB seeding
- Weather, time, and SQLite tool calls live
- Provider switch between Anthropic and OpenAI
- Token stats updating in real time
- SQL injection attempt being blocked

---

## 🔮 Future Enhancements

- **More Tools:** Calculator, currency converter, web search tool
- **Streaming Responses:** Stream tokens to the web UI as they arrive
- **Conversation Memory:** Persist multi-turn chat history across sessions
- **Model Benchmarking:** Compare latency and cost across providers side-by-side
- **Docker Support:** Containerise the agent for one-command deployment

---

## 📄 License

MIT — Free to use, modify, and distribute.
