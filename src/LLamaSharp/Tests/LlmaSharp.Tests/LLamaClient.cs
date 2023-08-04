using Microsoft.Extensions.Options;

namespace LlamaSharp.Tests;

public sealed class LLamaClient : ILLamaClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    private readonly LlamaClientOptions options;

    public LLamaClient(HttpClient httpClient, IOptions<LlamaClientOptions> options, ILogger logger)
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

    public Task GetModelOptions(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
        this.httpClient.Dispose();
    }
}
