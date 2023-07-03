using OneOf;
using OpenAI.Client.OpenAI.Models.Moderations;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IModerationAIClient
{
    Task<OneOf<ModerationResponse, ErrorResponse>> GetModerationAsync(ModerationRequest request, CancellationToken cancellationToken);
}
