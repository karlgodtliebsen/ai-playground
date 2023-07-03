using Microsoft.Extensions.Options;
using OneOf;
using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.FineTune;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class FineTuneAIClient : AIClientBase, IFineTuneAIClient
{

    public FineTuneAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<OneOf<FineTuneRequest, ErrorResponse>> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<FineTuneRequest, FineTuneRequest>(request.RequestUri, request, cancellationToken);
        return result;
    }


    public async Task<OneOf<FineTunes, ErrorResponse>> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<FineTuneRequest, FineTunes>(request.RequestUri, request, cancellationToken);
        return result;
    }


    public async Task<OneOf<FineTuneRequest, ErrorResponse>> RetrieveFineTuneAsync(FineTuneRequest request, string fineTuneId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<FineTuneRequest>($"{request.RequestUri}/{fineTuneId}", cancellationToken);
        return result;
    }

}
