using OneOf;

using OpenAI.Client.Models.FineTune;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IFineTuneAIClient
{

    Task<OneOf<FineTuneRequest, ErrorResponse>> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<OneOf<FineTunes, ErrorResponse>> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<OneOf<FineTuneRequest, ErrorResponse>> RetrieveFineTuneAsync(FineTuneRequest request, string fineTuneId, CancellationToken cancellationToken);

}
