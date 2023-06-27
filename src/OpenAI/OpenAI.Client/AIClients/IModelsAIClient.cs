using OneOf;

using OpenAI.Client.Models.Models;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IModelsAIClient
{
    Task<OneOf<Models.Models.Models, ErrorResponse>> GetModelsAsync(CancellationToken cancellationToken);

    Task<OneOf<Model, ErrorResponse>> GetModelAsync(string modelId, CancellationToken cancellationToken);
}
