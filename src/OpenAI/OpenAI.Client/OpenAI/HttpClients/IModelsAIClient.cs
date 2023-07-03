using OneOf;
using OpenAI.Client.OpenAI.Models.Models;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IModelsAIClient
{
    Task<OneOf<Models.Models.Models, ErrorResponse>> GetModelsAsync(CancellationToken cancellationToken);

    Task<OneOf<Model, ErrorResponse>> GetModelAsync(string modelId, CancellationToken cancellationToken);
}
