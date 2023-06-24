using OneOf;

using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IEditsAIClient
{
    Task<OneOf<Completions, ErrorResponse>> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken);
}
