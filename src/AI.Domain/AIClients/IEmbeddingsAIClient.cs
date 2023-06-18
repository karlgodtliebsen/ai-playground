using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface IEmbeddingsAIClient
{
    Task<Response<Embeddings>?> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken);
}