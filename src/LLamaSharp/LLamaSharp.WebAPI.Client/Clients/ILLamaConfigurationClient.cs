using LLamaSharp.WebAPI.Client.Clients.Models;

namespace LLamaSharp.WebAPI.Client.Clients;

public interface ILLamaConfigurationClient : IDisposable
{
    Task<string> GetPromptTemplatesAsync(CancellationToken cancellationToken);

    Task<LlamaModel> GetModelOptions(CancellationToken cancellationToken);

    Task<InferenceModel> GetInferenceOptions(CancellationToken cancellationToken);

    Task<IList<string>> GetModels(CancellationToken cancellationToken);

    Task<string> CheckHealthEndpoint(CancellationToken cancellationToken);

}
