using AI.Domain.Configuration;
using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;
using Microsoft.Extensions.Options;

namespace AI.Domain.AIClients.Implementation;

public class EditsAIClient : AIClientBase, IEditsAIClient
{

    public EditsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }


    public async Task<Response<Completions>?> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken)
    {

        var result = await PostAsync<EditsRequest, Completions>("edits", request, cancellationToken);
        return new Response<Completions>(result!);
    }
}