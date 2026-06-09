namespace ToolForge.Agent;

public class AgentResult
{
    public string Reply { get; set; } = "";

    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }

    public int ToolCalls { get; set; }
}