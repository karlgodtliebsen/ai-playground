using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

namespace LlamaSharp.Tests;

public interface ILLamaCompositeOperationsClient : IDisposable
{
    Task<string> InteractiveExecutorWithChat(ExecutorInferRequest request, CancellationToken cancellationToken);

}
