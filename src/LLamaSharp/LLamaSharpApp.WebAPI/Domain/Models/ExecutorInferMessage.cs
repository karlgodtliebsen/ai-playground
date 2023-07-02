using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Domain.Models;

/// <summary>
/// Domain Model for ExecutorInferMessage
/// </summary>
public class ExecutorInferMessage : SimpleTextMessage
{
    /// <summary>
    /// ExecutorInferMessage
    /// </summary>
    /// <param name="text"></param>
    public ExecutorInferMessage(string? text) : base(text)
    {
    }

    /// <summary>
    /// The request specific InferenceOptions: Optional
    /// </summary>
    public InferenceOptions? InferenceOptions { get; set; } = default!;

    /// <summary>
    /// The user id
    /// </summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Discriminator for the stateful Executor type
    /// Will be ignored when UseStatelessExecutor is true
    /// May be one of the following:InteractiveExecutor or InstructExecutor 
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InstructModeExecute.md"/>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InteractiveModeExecute.md"/>
    /// </summary>
    public InferenceType InferenceType { get; set; } = InferenceType.InteractiveExecutor;


    /// <summary>
    /// Use a stateless executor
    /// </summary>
    public bool UseStatelessExecutor { get; set; }

    /// <summary>
    /// Use a stateful executor, one of 'InteractiveExecutor' or 'InstructExecutor'
    /// </summary>
    public bool UsePersistedExecutorState { get; set; }
}


/// <summary>
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/InstructModeExecute.cs"/>
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/InteractiveModeExecute.cs"/>
/// </summary>
public enum InferenceType
{
    /// <summary>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InteractiveModeExecute.md"/>
    /// </summary>
    InteractiveExecutor,
    /// <summary>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InstructModeExecute.md"/>
    /// </summary>
    InstructExecutor
}
