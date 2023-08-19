using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

namespace LlamaSharp.Tests.Utils;

public interface ILLamaConfigurationClient : IDisposable
{
    Task<string> GetPromptTemplatesAsync(CancellationToken cancellationToken);

    Task<LlamaModelRequestResponse> GetModelOptions(CancellationToken cancellationToken);

    Task<InferenceRequestResponse> GetInferenceOptions(CancellationToken cancellationToken);

    Task<IList<string>> GetModels(CancellationToken cancellationToken);

    Task<string> CheckHealthEndpoint(CancellationToken cancellationToken);

}
