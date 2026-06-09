using ToolForge.Tools;
using Xunit;

namespace ToolForge.Tests;

// ── TimeTool Tests ─────────────────────────────────────────
public class TimeToolTests
{
    private readonly TimeTool _tool = new();

    [Fact]
    public void GetTime_ValidTimezone_ReturnsFormattedString()
    {
        var result = _tool.GetTime("Asia/Kolkata");
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Contains("IST", result);
    }

    [Fact]
    public void GetTime_UTC_ReturnsUtcTime()
    {
        var result = _tool.GetTime("UTC");
        Assert.Contains("UTC", result);
    }

    [Fact]
    public void GetTime_InvalidTimezone_ReturnsErrorString()
    {
        var result = _tool.GetTime("Mars/Olympus");
        Assert.Contains("Unknown timezone", result);
        Assert.DoesNotContain("Exception", result);
    }

    [Fact]
    public void GetTime_DefaultsToUtc()
    {
        var result = _tool.GetTime();
        Assert.Contains("UTC", result);
    }
}

// ── SqliteTool Tests ───────────────────────────────────────
public class SqliteToolTests : IAsyncLifetime
{
    private string _dbPath = string.Empty;
    private SqliteTool _tool = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.GetTempFileName() + ".db";
        await ToolForge.Database.SchemaSetup.InitializeAsync(_dbPath);
        _tool = new SqliteTool(_dbPath);
    }

    public Task DisposeAsync()
    {
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Query_ValidSelect_ReturnsJson()
    {
        var result = await _tool.QueryAsync("SELECT * FROM employees LIMIT 1");
        Assert.StartsWith("[", result); // JSON array
        Assert.Contains("name", result);
    }

    [Fact]
    public async Task Query_EmptyResult_ReturnsNoResultsMessage()
    {
        var result = await _tool.QueryAsync("SELECT * FROM employees WHERE name = 'Nobody123'");
        Assert.Equal("No results found.", result);
    }

    [Fact]
    public async Task Query_DropStatement_IsBlocked()
    {
        var result = await _tool.QueryAsync("DROP TABLE employees");
        Assert.Contains("Blocked", result);
    }

    [Fact]
    public async Task Query_DeleteStatement_IsBlocked()
    {
        var result = await _tool.QueryAsync("DELETE FROM employees");
        Assert.Contains("Only SELECT", result);
    }

    [Fact]
    public async Task Query_EmptyString_ReturnsError()
    {
        var result = await _tool.QueryAsync("");
        Assert.Contains("empty", result);
    }

    [Fact]
    public async Task Query_TopSalespeople_ReturnsOrdered()
    {
        var result = await _tool.QueryAsync(
            "SELECT name, sales FROM employees ORDER BY sales DESC LIMIT 3");
        Assert.Contains("Priya", result);
    }
}

// ── WeatherTool Tests ──────────────────────────────────────
// Note: These tests hit the real API. Set OPENWEATHER_API_KEY to run.
public class WeatherToolTests
{
    [Fact]
    public async Task GetWeather_EmptyCity_ThrowsArgumentException()
    {
        var tool = new WeatherTool();
        await Assert.ThrowsAsync<ArgumentException>(() => tool.GetWeatherAsync(""));
    }

    [Fact(Skip = "Requires live OPENWEATHER_API_KEY")]
    public async Task GetWeather_ValidCity_ReturnsTemperature()
    {
        var tool = new WeatherTool();
        var result = await tool.GetWeatherAsync("Chennai");
        Assert.Contains("Temp:", result);
        Assert.Contains("Humidity:", result);
    }

    [Fact(Skip = "Requires live OPENWEATHER_API_KEY")]
    public async Task GetWeather_InvalidCity_ReturnsUserFriendlyError()
    {
        var tool = new WeatherTool();
        var result = await tool.GetWeatherAsync("ZZZInvalidCity999");
        Assert.Contains("not found", result.ToLower());
        // Should NOT throw — agent needs a string, not an exception
    }
}
