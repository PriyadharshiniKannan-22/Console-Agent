namespace ToolForge.UI;

/// <summary>
/// Handles all console rendering with color-coded output.
/// </summary>
public static class ConsoleRenderer
{
    public static void ShowBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("""

          ████████╗ ██████╗  ██████╗ ██╗     ███████╗ ██████╗ ██████╗  ██████╗ ███████╗
             ██╔══╝██╔═══██╗██╔═══██╗██║     ██╔════╝██╔═══██╗██╔══██╗██╔════╝ ██╔════╝
             ██║   ██║   ██║██║   ██║██║     █████╗  ██║   ██║██████╔╝██║  ███╗█████╗
             ██║   ██║   ██║██║   ██║██║     ██╔══╝  ██║   ██║██╔══██╗██║   ██║██╔══╝
             ██║   ╚██████╔╝╚██████╔╝███████╗██║     ╚██████╔╝██║  ██║╚██████╔╝███████╗
             ╚═╝    ╚═════╝  ╚═════╝ ╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚══════╝
        """);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Tool-Calling AI Agent in C# | Team ToolForge");
        Console.WriteLine("  Tools: get_weather | get_time | query_sqlite");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void ShowPrompt()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("🤖 You > ");
        Console.ResetColor();
    }

    public static void ShowAnswer(string answer)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("💡 Agent > ");
        Console.ResetColor();
        Console.WriteLine(answer);
        Console.WriteLine();
    }

    public static void ShowError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Error] {error}");
        Console.ResetColor();
    }

    public static void ShowHelp()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Try: 'What's the weather in Mumbai?'");
        Console.WriteLine("       'What time is it in London?'");
        Console.WriteLine("       'Show me the top 3 employees by sales'");
        Console.WriteLine("       'List all electronics under 5000'");
        Console.WriteLine("  Type 'exit' to quit.\n");
        Console.ResetColor();
    }
}
