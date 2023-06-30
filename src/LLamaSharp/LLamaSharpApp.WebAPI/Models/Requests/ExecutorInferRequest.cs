namespace LLamaSharpApp.WebAPI.Models.Requests;

/// <summary>
/// Request object to hold the text/message to be sent to the executor
/// </summary>
public class ExecutorInferRequest : BaseMessageRequest
{
    /// <summary>
    /// When true, the models state will be loaded and saved from the file system
    /// </summary>
    public bool? UsePersistedModelState { get; set; }
    /// <summary>
    /// When true, the Executor state will be loaded and saved from the file system
    /// </summary>
    public bool? UsePersistedExecutorState { get; set; } = true;

    /// <summary>
    /// When true, the a Stateless Executor will be used
    /// </summary>
    public bool? UseStatelessExecutor { get; set; } = false;
}
