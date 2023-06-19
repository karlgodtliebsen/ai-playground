using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

public class FineTuneAIClient : AIClientBase, IFineTuneAIClient
{

    public FineTuneAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<Response<FineTuneRequest>?> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<FineTuneRequest, FineTuneRequest>(request.RequestUri, request, cancellationToken);
        return new Response<FineTuneRequest>(result!);
    }


    public async Task<Response<FineTunes>?> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<FineTuneRequest, FineTunes>(request.RequestUri, request, cancellationToken);
        return new Response<FineTunes>(result!);
    }


    public async Task<Response<FineTuneRequest>?> RetrieveFineTuneAsync(FineTuneRequest request, string fineTuneId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<FineTuneRequest>($"{request.RequestUri}/{fineTuneId}", cancellationToken);
        return new Response<FineTuneRequest>(result!);
    }

}