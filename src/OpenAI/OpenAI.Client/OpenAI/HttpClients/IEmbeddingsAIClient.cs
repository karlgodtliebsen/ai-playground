using OneOf;
using OpenAI.Client.OpenAI.Models.Embeddings;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IEmbeddingsAIClient
{
    Task<OneOf<Embeddings, ErrorResponse>> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken);
}
