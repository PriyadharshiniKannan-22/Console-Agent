using Microsoft.Data.Sqlite;

namespace ToolForge.Database;

public static class SchemaSetup
{
    public static async Task InitializeAsync(string dbPath)
    {
        await using var connection =
            new SqliteConnection($"Data Source={dbPath}");

        await connection.OpenAsync();

        var createEmployees = """
        CREATE TABLE IF NOT EXISTS Employees
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Department TEXT NOT NULL,
            Salary INTEGER NOT NULL
        );
        """;

        var createProducts = """
        CREATE TABLE IF NOT EXISTS Products
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Price REAL NOT NULL,
            Stock INTEGER NOT NULL
        );
        """;

        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = createEmployees;
            await cmd.ExecuteNonQueryAsync();
        }

        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = createProducts;
            await cmd.ExecuteNonQueryAsync();
        }

        await SeedData.SeedAsync(connection);
    }
}