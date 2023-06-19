using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

public class ModelsAIClient : AIClientBase, IModelsAIClient
{
    private const string uri = "models";

    public ModelsAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<Response<Models.Models>> GetModelsAsync(CancellationToken cancellationToken)
    {
        var result = await GetAsync<Models.Models>(uri, cancellationToken);
        return new Response<Models.Models>(result!);
    }

    public async Task<Response<Model>> GetModelAsync(string modelId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<Model>($"{uri}/{modelId}", cancellationToken);
        return new Response<Model>(result!);
    }


}