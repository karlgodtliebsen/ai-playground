using AI.Domain.Models;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface IModelsAIClient
{
    Task<Response<Models.Models>> GetModelsAsync(CancellationToken cancellationToken);

    Task<Response<Model>> GetModelAsync(string modelId, CancellationToken cancellationToken);
}