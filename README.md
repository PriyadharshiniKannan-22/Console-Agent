# AgentEngine

> Enterprise-grade AI Agent Platform built with C#, ASP.NET Core, OpenAI, Ollama, SQLite, and Next.js.

AgentEngine demonstrates how modern AI Agent architectures can be implemented beyond the Python ecosystem. The platform enables intelligent tool calling, multi-model orchestration, external API integration, and database interactions through a unified conversational interface.

---

# Business Problem

Most AI Agent frameworks and implementations are heavily focused on Python, making adoption difficult for organizations that already operate large-scale .NET ecosystems.

Many enterprise applications are built using:

- ASP.NET
- C#
- SQL Server
- Internal APIs
- Legacy business systems

Organizations need a way to integrate AI capabilities without rewriting their existing infrastructure.

AgentEngine demonstrates how modern AI Agent patterns can be implemented within the .NET ecosystem while maintaining enterprise compatibility.

---

# Features

## AI Agent Loop

Implements a complete reasoning workflow:

```text
User Request
      ↓
Agent Analysis
      ↓
Tool Selection
      ↓
Tool Execution
      ↓
Observation Collection
      ↓
Final Response
```

---

## Multi-LLM Support

### Primary Model

- OpenAI GPT

### Fallback Model

- Ollama (Local LLM)

If the primary provider becomes unavailable, AgentEngine automatically switches to the fallback provider.

---

## Dynamic Tool Calling

The agent can invoke tools dynamically based on user intent.

### Supported Tools

| Tool | Purpose |
|--------|----------|
| Time Tool | Retrieve current system time |
| Weather Tool | Fetch live weather information |
| Database Tool | Query internal SQLite database |
| System Tool | Retrieve system metadata |

---

## External API Integration

Real-world data retrieval through:

- OpenAI API
- OpenWeather API
- Future MCP Integrations

---

## Knowledge Layer

SQLite-powered data storage:

- Student records
- Internal knowledge
- Structured retrieval

---

## Interactive Web Dashboard

Modern glassmorphic dashboard featuring:

- Landing Page
- Chat Interface
- Tool Execution Monitoring
- Agent Logs
- Architecture Overview

---

## Live Deployment

### Frontend

```text
https://agentengine.vercel.app
```

### Backend

```text
https://agentengine-api.onrender.com
```

---

# Architecture

```text
                    ┌─────────────────────┐
                    │      Frontend       │
                    │  Next.js + React    │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ ASP.NET Core API    │
                    └──────────┬──────────┘
                               │
                        Agent Engine
                               │
          ┌────────────┬──────────────┬─────────────┐
          ▼            ▼              ▼             ▼
     Time Tool   Weather API    SQLite DB    Future MCP
          │            │              │
          └────────────┴──────────────┘
                               │
                               ▼
                     Primary LLM (OpenAI)
                               │
                        (Fallback)
                               ▼
                      Ollama (Local)
```

---

# Technology Stack

## Frontend

- Next.js
- React
- Tailwind CSS
- shadcn/ui

## Backend

- ASP.NET Core
- C#
- REST APIs

## Database

- SQLite

## AI Layer

- OpenAI
- Ollama

## Deployment

- Vercel
- Render

## Collaboration

- GitHub

---

# Project Structure

```text
agentengine-console-agent/
│
├── README.md
├── LICENSE
├── prompts.md
├── AI_USAGE_NOTE.md
│
├── docs/
│   ├── architecture.png
│   ├── demo-script.md
│   └── screenshots/
│
├── frontend/
│   ├── app/
│   ├── components/
│   ├── public/
│   └── package.json
│
├── backend/
│   ├── Controllers/
│   ├── Agent/
│   │     AgentLoop.cs
│   │     ToolRouter.cs
│   │
│   ├── Tools/
│   │     TimeTool.cs
│   │     WeatherTool.cs
│   │
│   └── Database/
│         DatabaseTool.cs
│         students.db
│
├── tests/
│
├── sample-data/
│
└── resumes/
```

---

# Installation

## Clone Repository

```bash
git clone https://github.com/your-org/agentengine-console-agent.git

cd agentengine-console-agent
```

---

## Backend Setup

```bash
cd backend

dotnet restore

dotnet run
```

Backend will run at:

```text
http://localhost:5000
```

---

## Frontend Setup

```bash
cd frontend

npm install

npm run dev
```

Frontend will run at:

```text
http://localhost:3000
```

---

# Environment Variables

Create a `.env` file.

```env
OPENAI_API_KEY=your_openai_api_key

OPENWEATHER_API_KEY=your_weather_api_key

OLLAMA_BASE_URL=http://localhost:11434
```

---

# Example Queries

### Time Query

```text
What time is it?
```

### Weather Query

```text
What's the weather in Chennai?
```

### Database Query

```text
Show all students.
```

### Analytics Query

```text
Who has the highest CGPA?
```

---

# Sample Agent Execution

## User Input

```text
What's the weather in Chennai?
```

## Agent Reasoning

```text
Thought:
The user is requesting weather information.

Action:
Invoke Weather Tool.

Observation:
Weather API returned:
30°C, Humidity 78%.

Final Answer:
Current weather in Chennai is 30°C with 78% humidity.
```

---

# API Endpoints

## Chat Endpoint

```http
POST /api/chat
```

### Request

```json
{
  "message": "What time is it?"
}
```

### Response

```json
{
  "response": "Current time is 10:30 AM"
}
```

---

## Weather Endpoint

```http
GET /api/weather?city=Chennai
```

---

## Database Endpoint

```http
GET /api/students
```

---

# AI Usage

## AI Tools Used

- ChatGPT
- GitHub Copilot

## AI Assisted With

- Code scaffolding
- API integration
- Agent workflow design
- Unit tests
- Documentation

## Human Contributions

- Architecture design
- Feature planning
- Integration
- Testing
- Deployment

---

# Assumptions

- Internet connection is required for OpenAI and Weather APIs.
- SQLite is used as the local knowledge layer.
- Ollama acts as a local fallback LLM.

---

# Limitations

- Limited number of tools in the current release.
- SQLite dataset is demo-sized.
- Fallback responses depend on local model quality.

---

# Future Enhancements

## MCP Integration

Support for Model Context Protocol tools.

## Additional LLM Providers

- Google Gemini
- Anthropic Claude
- Groq

## Vector Search

- ChromaDB
- PostgreSQL + pgvector

## Authentication

- Clerk
- OAuth

## Observability

- Agent traces
- Tool execution analytics
- Performance monitoring

---

# Team

**Project Name:** AgentEngine

**Repository:** `agentengine-console-agent`

**Team Name:** AgentForge

**Tagline:**

> Building enterprise AI agents beyond Python.

---

# License

This project is licensed under the MIT License.
