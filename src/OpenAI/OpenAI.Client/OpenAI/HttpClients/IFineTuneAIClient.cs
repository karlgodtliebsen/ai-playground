using OneOf;
using OpenAI.Client.OpenAI.Models.FineTune;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IFineTuneAIClient
{

    Task<OneOf<FineTuneRequest, ErrorResponse>> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<OneOf<FineTunes, ErrorResponse>> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<OneOf<FineTuneRequest, ErrorResponse>> RetrieveFineTuneAsync(FineTuneRequest request, string fineTuneId, CancellationToken cancellationToken);

}
