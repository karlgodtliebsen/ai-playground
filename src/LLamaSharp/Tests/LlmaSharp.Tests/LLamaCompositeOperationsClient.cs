using System.Net.Http.Json;

using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

using Microsoft.Extensions.Options;

namespace LlamaSharp.Tests;

public sealed class LLamaCompositeOperationsClient : ILLamaCompositeOperationsClient
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

    public async Task<string> InteractiveExecutorWithChat(ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"{options.Endpoint}/api/llama/composite/executeInstruction", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<string>(cancellationToken: cancellationToken);
        return result!;
    }


    public void Dispose()
    {
        this.httpClient.Dispose();
    }


}
