using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

public interface IExecutorService
{
    IAsyncEnumerable<string> Executor(ExecutorInferMessage input, CancellationToken cancellationToken);
}
