using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

public class EditsAIClient : AIClientBase, IEditsAIClient
{

    public EditsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }


    public async Task<OneOf<Completions, ErrorResponse>> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<EditsRequest, Completions>(request.RequestUri, request, cancellationToken);
        return result;
    }
}
