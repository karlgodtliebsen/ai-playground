using Microsoft.Extensions.Options;
using OneOf;
using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.Embeddings;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class EmbeddingsAIClient : AIClientBase, IEmbeddingsAIClient
{

    public EmbeddingsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIConfiguration> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }


    public async Task<OneOf<Embeddings, ErrorResponse>> GetEmbeddingsAsync(EmbeddingsRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<EmbeddingsRequest, Embeddings>(request.RequestUri, request, cancellationToken);
        return result;
    }
}
