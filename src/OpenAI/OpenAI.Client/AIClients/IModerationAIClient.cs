using OneOf;

using OpenAI.Client.Models.Moderations;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IModerationAIClient
{
    Task<OneOf<ModerationResponse, ErrorResponse>> GetModerationAsync(ModerationRequest request, CancellationToken cancellationToken);
}
