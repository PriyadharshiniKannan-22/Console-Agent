using ToolForge.Agent;
using ToolForge.Database;
using DotNetEnv;
DotNetEnv.Env.Load();
var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
var builder = WebApplication.CreateBuilder(args);

// ── Services ───────────────────────────────────────────────
builder.Services.AddControllers();

// Register ToolRegistry and AgentLoop as singletons (one DB, one agent)
var dbPath = Path.Combine(AppContext.BaseDirectory, "toolforge.db");
builder.Services.AddSingleton(new ToolRegistry(dbPath));
builder.Services.AddSingleton<AgentLoop>();

// Allow the frontend to call the API from the same origin (or any for dev)
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Init DB ────────────────────────────────────────────────
await SchemaSetup.InitializeAsync(dbPath);

// ── Middleware ─────────────────────────────────────────────
app.UseCors();
app.UseDefaultFiles();          // serves index.html from wwwroot/
app.UseStaticFiles();           // serves web/ assets

app.MapControllers();

app.Run();
