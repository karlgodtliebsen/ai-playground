using Microsoft.Extensions.Options;
using OneOf;
using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class EditsAIClient : AIClientBase, IEditsAIClient
{

    public EditsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIConfiguration> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }


    public async Task<OneOf<Completions, ErrorResponse>> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<EditsRequest, Completions>(request.RequestUri, request, cancellationToken);
        return result;
    }
}
