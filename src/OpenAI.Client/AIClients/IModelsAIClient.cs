using OpenAI.Client.Models;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IModelsAIClient
{
    Task<Response<Models.Models>> GetModelsAsync(CancellationToken cancellationToken);

    Task<Response<Model>> GetModelAsync(string modelId, CancellationToken cancellationToken);
}