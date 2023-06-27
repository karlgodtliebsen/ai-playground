using OneOf;

using OpenAI.Client.Models.Embeddings;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IEmbeddingsAIClient
{
    Task<OneOf<Embeddings, ErrorResponse>> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken);
}
