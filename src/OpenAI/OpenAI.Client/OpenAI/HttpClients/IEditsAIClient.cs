using OneOf;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IEditsAIClient
{
    Task<OneOf<Completions, ErrorResponse>> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken);
}
