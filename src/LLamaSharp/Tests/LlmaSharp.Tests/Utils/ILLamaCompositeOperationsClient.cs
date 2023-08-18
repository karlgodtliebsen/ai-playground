using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

namespace LlamaSharp.Tests.Utils;

public interface ILLamaCompositeOperationsClient : IDisposable
{
    Task<string> InteractiveExecutorWithChatAndNoRoleNames(ExecutorInferRequest request, CancellationToken cancellationToken);
    Task<string> InteractiveExecutorWithChatAndRoleNames(ExecutorInferRequest request, CancellationToken cancellationToken);

    Task<string> ExecuteInstructions(ExecutorInferRequest request, CancellationToken cancellationToken);

    Task<string> InteractiveExecuteInstructions(ExecutorInferRequest request, CancellationToken cancellationToken);

    Task<float[]> GetEmbeddings(EmbeddingsRequest request, CancellationToken cancellationToken);
}
