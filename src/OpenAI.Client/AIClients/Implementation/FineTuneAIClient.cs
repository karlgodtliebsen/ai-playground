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

    public async Task<Response<FineTune>?> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<FineTuneRequest, FineTune>($"fine-tunes", request, cancellationToken);
        return new Response<FineTune>(result!);
    }


    public async Task<Response<FineTunes>?> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<FineTuneRequest, FineTunes>($"fine-tunes", request, cancellationToken);
        return new Response<FineTunes>(result!);
    }


    public async Task<Response<FineTune>?> RetrieveFineTuneAsync(string fineTuneId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<FineTune>($"fine-tunes/{fineTuneId}", cancellationToken);
        return new Response<FineTune>(result!);
    }

}