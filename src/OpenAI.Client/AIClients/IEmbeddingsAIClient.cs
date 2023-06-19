using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IEmbeddingsAIClient
{
    Task<Response<Embeddings>?> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken);
}