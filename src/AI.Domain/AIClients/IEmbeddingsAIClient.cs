using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface IEmbeddingsAIClient
{
    Task<Response<Embeddings>?> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken);
}