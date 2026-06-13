# 🔧 ToolForge Agent

### A Tool-Calling AI Agent in C# using ASP.NET Core, Groq, and SQLite

> *"AI agents and tool calling are not limited to Python — they can be built effectively in C# too."*

---

## 🧠 What Is This?

**ToolForge Agent** is a tool-calling AI assistant built entirely in **C# (.NET 8)** that demonstrates how Large Language Models can interact with external tools through a custom agent loop.

The application accepts natural language queries through a web interface, allows the LLM to decide when tools are required, executes those tools, and returns grounded responses based on real-world data.

The project showcases:

* Function / Tool Calling
* Agent Loop Architecture
* External API Integration
* SQLite Database Querying
* ASP.NET Core Web APIs
* Token Usage Tracking

Unlike many agent implementations that rely heavily on Python frameworks, ToolForge demonstrates that modern AI agents can be implemented cleanly using the .NET ecosystem.

---

# ✨ Features

| Feature                     | Description                                         |
| --------------------------- | --------------------------------------------------- |
| 🔁 Tool-Calling Agent Loop  | LLM decides when a tool should be invoked           |
| 🌦️ Live Weather Tool       | Retrieves current weather using OpenWeatherMap      |
| 🕒 Timezone-Aware Time Tool | Returns current time for any valid timezone         |
| 🗄️ SQLite Database Tool    | Retrieves information from a local SQLite database  |
| 🌐 ASP.NET Core API         | Backend API serving chat requests                   |
| 💻 Web Interface            | Browser-based terminal-style chat UI                |
| 📊 Token Tracking           | Tracks input tokens, output tokens, and tool calls  |
| 💰 Cost Estimation          | Estimates API usage cost based on token consumption |
| 🔒 Safe Database Access     | Restricts queries to SELECT statements only         |
| ⚡ Groq-Powered Inference    | Fast LLM inference using Groq                       |

---

# 🏗️ System Architecture

```text
User
 │
 ▼
Web Interface (HTML + Vanilla JS)
 │
 ▼
ASP.NET Core API
 (/api/chat)
 │
 ▼
Agent Loop
 │
 ▼
Groq LLM
(Llama 4 Scout)
 │
 ├─────────────┬─────────────┬─────────────┐
 ▼             ▼             ▼
get_time()   get_weather()  query_sqlite()
 │             │             │
 ▼             ▼             ▼
Timezone    OpenWeather    SQLite
Service       API         Database
```

---

# 🔄 Agent Workflow

### Step 1 — User Query

User enters a natural language question.

Example:

```text
What time is it in Asia/Kolkata?
```

---

### Step 2 — LLM Analysis

The model receives:

* System Prompt
* User Query
* Available Tool Definitions

The model determines whether a tool is required.

---

### Step 3 — Tool Invocation

Example:

```json
{
  "name": "get_time",
  "arguments": {
    "timezone": "Asia/Kolkata"
  }
}
```

---

### Step 4 — Tool Execution

ToolRegistry executes the requested tool.

Example result:

```text
Asia/Kolkata: 2026-06-09 21:04:39
```

---

### Step 5 — Grounded Response

Tool result is fed back to the LLM.

Final response:

```text
It is 2026-06-09 21:04:39 in Asia/Kolkata.
```

---

# 🛠️ Technology Stack

| Category         | Technology                    |
| ---------------- | ----------------------------- |
| Language         | C#                            |
| Framework        | ASP.NET Core (.NET 8)         |
| LLM Provider     | Groq                          |
| Model            | Llama 4 Scout (17B)           |
| Database         | SQLite                        |
| Weather API      | OpenWeatherMap                |
| Frontend         | HTML, CSS, Vanilla JavaScript |
| Timezone Support | TimeZoneConverter             |
| Configuration    | DotNetEnv                     |
| Version Control  | Git + GitHub                  |

---

# 📁 Project Structure

```text
Project/
│
├── api/
│   ├── ChatController.cs
│   ├── Startup.cs
│   └── Models/
│       └── ChatModels.cs
│
├── src/
│   ├── Agent/
│   │   ├── AgentLoop.cs
│   │   ├── AgentResult.cs
│   │   ├── PromptBuilder.cs
│   │   └── ToolRegistry.cs
│   │
│   ├── Database/
│   │   ├── SchemaSetup.cs
│   │   └── SeedData.cs
│   │
│   └── Tools/
│       ├── SqliteTool.cs
│       ├── TimeTool.cs
│       └── WeatherTool.cs
│
├── wwwroot/
│   ├── index.html
│   └── js/
│       ├── agent.js
│       └── tokenStats.js
│
├── docs/
│   └── README.md
│
├── prompts/
│   └── prompts.md
│
├── sample-data/
│   ├── sample-queries.txt
│   └── expected-outputs.md
│
├── .env.example
├── .gitignore
├── AgentEngine.csproj
├── ConsoleAgent.sln
└── README.md
```

---

# 🧩 Core Components

## AgentLoop.cs

The heart of the application.

Responsibilities:

* Builds message history
* Sends requests to Groq
* Handles tool calls
* Executes tools
* Maintains conversation state
* Tracks token usage
* Returns final answers

This file implements the complete agent reasoning cycle.

---

## PromptBuilder.cs

Provides the system prompt.

Defines rules such as:

```text
If user asks for time → call get_time

If user asks for weather → call get_weather

If user asks about employees or products → call query_sqlite
```

---

## ToolRegistry.cs

Acts as the central tool dispatcher.

Responsibilities:

* Registers all tools
* Defines tool schemas
* Executes tool requests
* Routes tool results back to the agent

---

## ChatController.cs

API entry point.

Endpoints:

```text
POST /api/chat
GET  /api/health
```

Handles communication between frontend and backend.

---

## SqliteTool.cs

Provides database access.

Supports:

```sql
SELECT * FROM Employees
SELECT * FROM Products
```

Safety features:

* Empty query validation
* SELECT-only restriction
* 3-second timeout
* 200-row result limit

---

## WeatherTool.cs

Fetches weather data from OpenWeatherMap.

Returns:

* Temperature
* Humidity
* Weather Condition

---

## TimeTool.cs

Returns current time for any valid timezone.

Example:

```text
Asia/Kolkata
Europe/London
America/New_York
```

Uses TimeZoneConverter for cross-platform compatibility.

---

# 🗄️ Database Schema

## Employees

| Column     | Type    |
| ---------- | ------- |
| Id         | Integer |
| Name       | Text    |
| Department | Text    |
| Salary     | Integer |

Sample Records:

```text
Alice    Engineering   85000
Bob      Marketing     65000
Charlie  Sales         70000
Diana    Engineering   95000
```

---

## Products

| Column | Type    |
| ------ | ------- |
| Id     | Integer |
| Name   | Text    |
| Price  | Real    |
| Stock  | Integer |

Sample Records:

```text
Laptop   65000   10
Mouse      500  100
Keyboard  1500   50
Monitor  12000   20
```

---

# 🚀 Running the Project

## 1. Configure Environment Variables

Create a `.env` file:

```env
GROQ_API_KEY=your_key_here
OPENWEATHER_API_KEY=your_key_here
```

---

## 2. Restore Packages

```bash
dotnet restore
```

---

## 3. Build

```bash
dotnet build
```

---

## 4. Run

```bash
dotnet run
```

---

## 5. Open Browser

```text
http://localhost:5000
```

(or the URL displayed in the terminal)

---

# 🧪 Sample Queries

### Time Tool

```text
What time is it in Asia/Kolkata?
```

Expected Tool:

```text
get_time
```

---

### Weather Tool

```text
What's the weather in Chennai?
```

Expected Tool:

```text
get_weather
```

---

### Database Tool

```text
Show all employees
```

Expected Tool:

```text
query_sqlite
```

---

### Product Query

```text
List all products
```

Expected Tool:

```text
query_sqlite
```

---

# 📊 Token Statistics

The web interface tracks:

| Metric         | Description                    |
| -------------- | ------------------------------ |
| Input Tokens   | Tokens sent to the model       |
| Output Tokens  | Tokens generated by the model  |
| Tool Calls     | Number of tool invocations     |
| Messages       | Number of chat exchanges       |
| Estimated Cost | Calculated from pricing tables |

---

# ⚠️ Current Limitations

* No conversation memory
* Single-user SQLite database
* Weather depends on OpenWeatherMap availability
* Tool selection depends on model behavior
* No authentication system
* No streaming responses

---

# 🔮 Future Enhancements

### Additional Tools

* Calculator Tool
* Currency Converter
* Web Search Tool

### Memory

* Persistent conversation history

### Streaming

* Real-time token streaming

### Authentication

* User login and session management

### Multiple Models

* Dynamic model switching

### Advanced Analytics

* Latency tracking
* Tool performance metrics
* Cost reporting dashboard

---

# 🎯 Key Takeaway

> **ToolForge demonstrates that modern AI agents and tool-calling workflows can be implemented effectively in C# using ASP.NET Core, Groq, and SQLite—without relying on Python-specific agent frameworks.**

---

## 👥 Team

**ToolForge**

Built as a demonstration of tool-calling AI agents in the .NET ecosystem.

---
