using AI.Domain.Configuration;
using AI.Domain.Models;

using Microsoft.Extensions.Options;

namespace AI.Domain.AIClients;

public class EmbeddingsAIClient : AIClientBase, IEmbeddingsAIClient
{

    public EmbeddingsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }


    public async Task<Response<Embeddings>?> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<EmbeddingsRequest, Embeddings>("embeddings", request, cancellationToken);
        return new Response<Embeddings>(result!);
    }
}