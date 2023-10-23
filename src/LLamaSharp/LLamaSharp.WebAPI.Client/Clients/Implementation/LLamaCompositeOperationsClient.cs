using System.Net.Http.Json;

using LLamaSharp.WebAPI.Client.Clients.Models;

using Microsoft.Extensions.Options;

namespace LLamaSharp.WebAPI.Client.Clients.Implementation;

internal sealed class LLamaCompositeOperationsClient : ILLamaCompositeOperationsClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    private readonly LlamaClientOptions options;

    public LLamaCompositeOperationsClient(HttpClient httpClient, IOptions<LlamaClientOptions> options, ILogger logger)
    {
        this.logger = logger;
        this.options = options.Value;
        this.httpClient = httpClient;
    }

    public async Task<string> InteractiveExecutorWithChatAndNoRoleNames(ExecutorInferModel request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{options.Endpoint}/api/llama/composite/interactiveInstructionExecute/noroles", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<string> InteractiveExecutorWithChatAndRoleNames(ExecutorInferModel request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{options.Endpoint}/api/llama/composite/interactiveInstructionExecute/roles", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<string> ExecuteInstructions(ExecutorInferModel request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{options.Endpoint}/api/llama/execute/instructions", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<string> InteractiveExecuteInstructions(ExecutorInferModel request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{options.Endpoint}/api/llama/interactive/execute/instructions", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        return result!;
    }

    public async Task<float[]> GetEmbeddings(EmbeddingsModel request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{options.Endpoint}/api/llama/embeddings", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<float[]>(cancellationToken: cancellationToken);
        return result!;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }


}
