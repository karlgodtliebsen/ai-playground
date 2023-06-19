using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IFineTuneAIClient
{

    Task<Response<FineTune>?> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTunes>?> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTune>?> RetrieveFineTuneAsync(string fineTuneId, CancellationToken cancellationToken);

}