namespace LlamaSharp.Tests;

public interface ILLamaClient : IDisposable
{
    Task<string> GetPromptTemplatesAsync(CancellationToken cancellationToken);

    Task GetModelOptions(CancellationToken cancellationToken);

    Task<string> CheckHealthEndpoint(CancellationToken cancellationToken);

}
