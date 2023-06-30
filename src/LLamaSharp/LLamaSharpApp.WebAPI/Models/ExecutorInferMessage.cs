namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for ExecutorInferMessage
/// </summary>
public class ExecutorInferMessage : SimpleTextMessage
{
    public ExecutorInferMessage(string? text) : base(text)
    {
    }

    public bool UseStatelessExecutor { get; set; }

    public bool UsePersistedExecutorState { get; set; }
}
