using Microsoft.Data.Sqlite;
using ToolForge.Database;
using ToolForge.Tools;
using Xunit;

namespace ToolForge.Tests;

/// <summary>
/// Integration tests for SqliteTool.
/// Each test gets a fresh in-memory (or temp-file) SQLite database
/// seeded with the standard employees + products data.
///
/// Covers:
///   - Valid SELECT queries return JSON
///   - SQL injection / destructive SQL is blocked
///   - Edge cases: empty query, no results, bad syntax
///   - Business queries match expected data
/// </summary>
public class SqliteToolTests : IAsyncLifetime
{
    private string _dbPath = string.Empty;
    private SqliteTool _tool = null!;

    // ── Setup / Teardown ─────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        // Use a temp file so Microsoft.Data.Sqlite can seed it properly
        _dbPath = Path.GetTempFileName() + ".db";
        await SchemaSetup.InitializeAsync(_dbPath);
        _tool = new SqliteTool(_dbPath);
    }

    public Task DisposeAsync()
    {
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        return Task.CompletedTask;
    }

    // ── Happy path ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Query_SelectAll_ReturnsJsonArray()
    {
        var result = await _tool.QueryAsync("SELECT * FROM employees LIMIT 1");

        Assert.StartsWith("[", result);
        Assert.EndsWith("]", result);
        Assert.Contains("name", result);
        Assert.Contains("dept", result);
    }

    [Fact]
    public async Task Query_TopSalespeople_ReturnsCorrectOrder()
    {
        var result = await _tool.QueryAsync(
            "SELECT name, sales FROM employees ORDER BY sales DESC LIMIT 3");

        // Priya has the highest sales in seed data (142000)
        var idx_priya = result.IndexOf("Priya", StringComparison.OrdinalIgnoreCase);
        var idx_arjun = result.IndexOf("Arjun", StringComparison.OrdinalIgnoreCase);

        Assert.True(idx_priya >= 0, "Priya should appear in top 3");
        Assert.True(idx_arjun >= 0, "Arjun should appear in top 3");
        Assert.True(idx_priya < idx_arjun, "Priya should appear before Arjun (higher sales)");
    }

    [Fact]
    public async Task Query_ProductsByCategory_FiltersCorrectly()
    {
        var result = await _tool.QueryAsync(
            "SELECT name, price FROM products WHERE category = 'Electronics' ORDER BY price ASC");

        Assert.Contains("Wireless Mouse", result);
        Assert.DoesNotContain("Standing Desk", result);
        Assert.DoesNotContain("Office Chair",  result);
    }

    [Fact]
    public async Task Query_AggregateCount_Works()
    {
        var result = await _tool.QueryAsync(
            "SELECT COUNT(*) as total FROM employees");

        Assert.Contains("total", result);
        // Seed data has 7 employees
        Assert.Contains("7", result);
    }

    [Fact]
    public async Task Query_JoinLike_CrossTableFilter_Works()
    {
        var result = await _tool.QueryAsync(
            "SELECT name FROM employees WHERE dept = 'Sales' ORDER BY name");

        Assert.Contains("Priya", result);
        Assert.Contains("Arjun", result);
        // Non-sales members should not appear
        Assert.DoesNotContain("Vikram", result);
    }

    [Fact]
    public async Task Query_EmptyResultSet_ReturnsNoResultsMessage()
    {
        var result = await _tool.QueryAsync(
            "SELECT * FROM employees WHERE name = '__nobody__'");

        Assert.Equal("No results found.", result);
    }

    [Fact]
    public async Task Query_CaseInsensitiveColumnNames_Work()
    {
        var result = await _tool.QueryAsync(
            "SELECT NAME, DEPT FROM employees LIMIT 2");

        Assert.StartsWith("[", result);
    }

    // ── SQL safety / injection guard ─────────────────────────────────────────

    [Theory]
    [InlineData("DROP TABLE employees")]
    [InlineData("DELETE FROM employees")]
    [InlineData("INSERT INTO employees (name) VALUES ('hacker')")]
    [InlineData("UPDATE employees SET sales = 0")]
    [InlineData("ALTER TABLE employees ADD COLUMN pwned TEXT")]
    [InlineData("TRUNCATE TABLE employees")]
    public async Task Query_DestructiveSQL_IsBlocked(string sql)
    {
        var result = await _tool.QueryAsync(sql);

        // Must return an error string, never throw, never mutate
        Assert.True(
            result.Contains("Only SELECT", StringComparison.OrdinalIgnoreCase) ||
            result.Contains("Blocked",     StringComparison.OrdinalIgnoreCase),
            $"Expected a block message but got: {result}");

        // Verify employees table is untouched
        var count = await _tool.QueryAsync("SELECT COUNT(*) as n FROM employees");
        Assert.Contains("7", count); // all 7 seed rows intact
    }

    [Fact]
    public async Task Query_SelectWithEmbeddedDrop_IsBlocked()
    {
        // SQL injection style: SELECT wrapping a DROP
        var result = await _tool.QueryAsync(
            "SELECT * FROM employees; DROP TABLE employees;--");

        Assert.True(
            result.Contains("Blocked", StringComparison.OrdinalIgnoreCase) ||
            result.Contains("Only SELECT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Query_AttachKeyword_IsBlocked()
    {
        var result = await _tool.QueryAsync("ATTACH DATABASE '/etc/passwd' AS secret");
        Assert.Contains("Only SELECT", result, StringComparison.OrdinalIgnoreCase);
    }

    // ── Edge cases ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Query_EmptyString_ReturnsEmptyError()
    {
        var result = await _tool.QueryAsync("");
        Assert.Contains("empty", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Query_WhitespaceOnly_ReturnsEmptyError()
    {
        var result = await _tool.QueryAsync("   \t\n  ");
        Assert.Contains("empty", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Query_SyntaxError_ReturnsUserFriendlyError()
    {
        var result = await _tool.QueryAsync("SELECT FROM WHERE");
        // Should not throw — must return a readable error string
        Assert.Contains("failed", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Unhandled", result);
    }

    [Fact]
    public async Task Query_NonExistentTable_ReturnsError()
    {
        var result = await _tool.QueryAsync("SELECT * FROM ghosts");
        Assert.Contains("failed", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Query_DoesNotThrow_ReturnsStringAlways()
    {
        // Anything that goes wrong must be surfaced as a string, never an exception.
        // The agent loop depends on this contract.
        var badInputs = new[]
        {
            "SELECT * FROM employees WHERE sales > 'not-a-number'",
            "SELECT name FROM",
            "SELECT 1/0",
        };

        foreach (var sql in badInputs)
        {
            var ex = await Record.ExceptionAsync(() => _tool.QueryAsync(sql));
            Assert.Null(ex); // must not throw
        }
    }
}
