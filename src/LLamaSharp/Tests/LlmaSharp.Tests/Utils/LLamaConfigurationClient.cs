using System.Net.Http.Json;

using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

using Microsoft.Extensions.Options;

namespace LlamaSharp.Tests.Utils;

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

    public async Task<LlamaModelRequestResponse> GetModelOptions(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/api/llama/configuration/modelparams", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LlamaModelRequestResponse>(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<InferenceRequestResponse> GetInferenceOptions(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"{options.Endpoint}/api/llama/configuration/inference", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<InferenceRequestResponse>(cancellationToken: cancellationToken);
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
