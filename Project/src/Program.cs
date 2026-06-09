using ToolForge.Agent;
DotNetEnv.Env.Load();
Console.WriteLine("====================================");
Console.WriteLine("          ToolForge");
Console.WriteLine(" AI Tool Calling Console Agent");
Console.WriteLine("Type 'exit' to quit.");
Console.WriteLine("====================================");

var dbPath = Path.Combine(AppContext.BaseDirectory, "toolforge.db");

var tools = new ToolRegistry(dbPath);
var agent = new AgentLoop(tools);

while (true)
{
    Console.Write("\nYou > ");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    try
    {
        var response = await agent.RunAsync(input);
        Console.WriteLine($"\nAgent > {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError > {ex.Message}");
    }
}