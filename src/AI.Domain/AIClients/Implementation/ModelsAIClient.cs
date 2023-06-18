using AI.Domain.Configuration;
using AI.Domain.Models;
using AI.Domain.Models.Responses;

using Microsoft.Extensions.Options;

namespace AI.Domain.AIClients.Implementation;

public class ModelsAIClient : AIClientBase, IModelsAIClient
{

    public ModelsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<Response<Models.Models>> GetModelsAsync(CancellationToken cancellationToken)
    {
        var result = await GetAsync<Models.Models>("models", cancellationToken);
        return new Response<Models.Models>(result!);
    }

    public async Task<Response<Model>> GetModelAsync(string modelId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<Model>($"models/{modelId}", cancellationToken);
        return new Response<Model>(result!);
    }


}