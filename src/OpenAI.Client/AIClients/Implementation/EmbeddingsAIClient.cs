using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models.Embeddings;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

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
        var result = await PostAsync<EmbeddingsRequest, Embeddings>(request.RequestUri, request, cancellationToken);
        return new Response<Embeddings>(result!);
    }
}