using Microsoft.Extensions.Options;
using OneOf;
using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.Models;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

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

    public async Task<OneOf<Models.Models.Models, ErrorResponse>> GetModelsAsync(CancellationToken cancellationToken)
    {
        var result = await GetAsync<Models.Models.Models>(uri, cancellationToken);
        return result;
    }

    public async Task<OneOf<Model, ErrorResponse>> GetModelAsync(string modelId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<Model>($"{uri}/{modelId}", cancellationToken);
        return result;
    }


}
