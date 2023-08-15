using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;

namespace LLamaSharp.Domain.Domain.Services;

/// <summary>
/// Interface for the Executor Service
/// </summary>
public interface IInteractiveExecutorService
{
    /// <summary>
    /// Execute the instructions, with the options to select either interactive or Instruction based execution
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> Execute(ExecutorInferMessage input, CancellationToken cancellationToken);

    IEnumerable<string> ChatUsingInteractiveExecutorWithTransformation(InferenceOptions inferenceOptions, LlamaModelOptions modelOptions, KeywordTextOutputStreamTransform executionOptions, string userInput);

    IEnumerable<string> ChatUsingInteractiveExecutor(InferenceOptions inferenceOptions, LlamaModelOptions modelOptions, string userInput);

    IAsyncEnumerable<string> InteractiveExecuteInstructions(ExecutorInferMessage input, CancellationToken cancellationToken);

    IAsyncEnumerable<string> ExecuteInstructions(ExecutorInferMessage input, CancellationToken cancellationToken);


}
