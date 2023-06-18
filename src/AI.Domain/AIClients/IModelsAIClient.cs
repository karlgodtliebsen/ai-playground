using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface IModelsAIClient
{
    Task<Response<Models.Models>> GetModelsAsync(CancellationToken cancellationToken);

    Task<Response<Model>> GetModelAsync(string modelId, CancellationToken cancellationToken);
}