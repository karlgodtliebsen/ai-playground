using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;

namespace LLamaSharp.Domain.Domain.Services;

/// <summary>
/// Interface for the Executor Service
/// </summary>
public interface IExecutorService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> Executor(ExecutorInferMessage input, CancellationToken cancellationToken);

    IEnumerable<string> ExecutorWithTransformation(InferenceOptions inferenceOptions, LlamaModelOptions modelOptions, string userInput);

    IEnumerable<string> ChatUsingInteractiveExecutor(InferenceOptions inferenceOptions, LlamaModelOptions modelOptions, string userInput);
}
