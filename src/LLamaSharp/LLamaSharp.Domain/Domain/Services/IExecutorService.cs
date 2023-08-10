using LLamaSharpApp.WebAPI.Domain.Models;

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
}
