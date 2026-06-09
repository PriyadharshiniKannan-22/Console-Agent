using Microsoft.Data.Sqlite;

namespace ToolForge.Database;

public static class SeedData
{
    public static async Task SeedAsync(SqliteConnection connection)
    {
        var countCmd = connection.CreateCommand();

        countCmd.CommandText =
            "SELECT COUNT(*) FROM Employees";

        var employeeCount =
            Convert.ToInt32(
                await countCmd.ExecuteScalarAsync()
            );

        if (employeeCount == 0)
        {
            var insertEmployees = connection.CreateCommand();

            insertEmployees.CommandText =
            """
            INSERT INTO Employees
            (Name, Department, Salary)
            VALUES
            ('Alice','Engineering',85000),
            ('Bob','Marketing',65000),
            ('Charlie','Sales',70000),
            ('Diana','Engineering',95000);
            """;

            await insertEmployees.ExecuteNonQueryAsync();
        }

        countCmd.CommandText =
            "SELECT COUNT(*) FROM Products";

        var productCount =
            Convert.ToInt32(
                await countCmd.ExecuteScalarAsync()
            );

        if (productCount == 0)
        {
            var insertProducts = connection.CreateCommand();

            insertProducts.CommandText =
            """
            INSERT INTO Products
            (Name, Price, Stock)
            VALUES
            ('Laptop',65000,10),
            ('Mouse',500,100),
            ('Keyboard',1500,50),
            ('Monitor',12000,20);
            """;

            await insertProducts.ExecuteNonQueryAsync();
        }
    }
}