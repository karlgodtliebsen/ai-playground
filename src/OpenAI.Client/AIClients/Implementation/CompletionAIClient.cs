using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

public class CompletionAIClient : AIClientBase, ICompletionAIClient
{

    public CompletionAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<Response<Completions>?> GetCompletionsAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<CompletionRequest, Completions>(request.RequestUri, request, cancellationToken);
        return new Response<Completions>(result!);
    }
}