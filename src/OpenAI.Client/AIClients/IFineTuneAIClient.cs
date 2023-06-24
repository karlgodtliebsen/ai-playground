using OpenAI.Client.Models.FineTune;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IFineTuneAIClient
{

    Task<Response<FineTuneRequest>?> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTunes>?> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTuneRequest>?> RetrieveFineTuneAsync(FineTuneRequest request, string fineTuneId, CancellationToken cancellationToken);

}