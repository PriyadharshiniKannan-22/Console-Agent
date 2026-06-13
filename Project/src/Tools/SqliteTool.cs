using Microsoft.Data.Sqlite;
using System.Text;

namespace ToolForge.Tools;

public class SqliteTool
{
    private readonly string _dbPath;

    public SqliteTool(string dbPath)
    {
        _dbPath = dbPath;
    }

    public async Task<string> QueryAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return "Query cannot be empty.";

        if (!query.TrimStart()
                 .StartsWith("SELECT",
                     StringComparison.OrdinalIgnoreCase))
        {
            return "Only SELECT statements are allowed.";
        }

        try
        {
            await using var connection =
                new SqliteConnection(
                    $"Data Source={_dbPath}"
                );

            await connection.OpenAsync();

            await using var command =
                new SqliteCommand(query, connection);

            command.CommandTimeout = 3;

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!reader.HasRows)
                return "No results found.";

            var sb = new StringBuilder();

            int rowCount = 0;

            while (await reader.ReadAsync())
            {
                if (rowCount >= 200)
                {
                    sb.AppendLine(
                        "... Results truncated (200 row limit reached)"
                    );
                    break;
                }

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    sb.Append(
                        $"{reader.GetName(i)}={reader[i]} "
                    );
                }

                sb.AppendLine();

                rowCount++;
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Database error: {ex.Message}";
        }
    }
}