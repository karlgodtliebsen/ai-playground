using System.Net.Http.Json;
using LLamaSharp.WebAPI.Client.Clients.Models;
using Microsoft.Extensions.Options;

namespace LLamaSharp.WebAPI.Client.Clients.Implementation;

public sealed class LLamaConfigurationClient : ILLamaConfigurationClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    private readonly LlamaClientOptions options;

    public LLamaConfigurationClient(HttpClient httpClient, IOptions<LlamaClientOptions> options, ILogger logger)
    {
        this.logger = logger;
        this.options = options.Value;
        this.httpClient = httpClient;
    }

    public async Task<string> GetPromptTemplatesAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/api/llama/configuration/prompt-templates", cancellationToken);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken);
        return text;
    }

    public async Task<LlamaModel> GetModelOptions(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/api/llama/configuration/modelparams", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LlamaModel>(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<InferenceModel> GetInferenceOptions(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/api/llama/configuration/inference", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<InferenceModel>(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<IList<string>> GetModels(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/api/llama/configuration/models", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IList<string>>(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<string> CheckHealthEndpoint(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/health", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        return result;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}
