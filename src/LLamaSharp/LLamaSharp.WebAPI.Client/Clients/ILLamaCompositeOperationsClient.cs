using LLamaSharp.WebAPI.Client.Clients.Models;

namespace LLamaSharp.WebAPI.Client.Clients;

public interface ILLamaCompositeOperationsClient : IDisposable
{
    Task<string> InteractiveExecutorWithChatAndNoRoleNames(ExecutorInferModel request, CancellationToken cancellationToken);
    Task<string> InteractiveExecutorWithChatAndRoleNames(ExecutorInferModel request, CancellationToken cancellationToken);

    Task<string> ExecuteInstructions(ExecutorInferModel request, CancellationToken cancellationToken);

    Task<string> InteractiveExecuteInstructions(ExecutorInferModel request, CancellationToken cancellationToken);

    Task<float[]> GetEmbeddings(EmbeddingsModel request, CancellationToken cancellationToken);
}
