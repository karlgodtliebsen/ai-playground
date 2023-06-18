using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface IFineTuneAIClient
{

    Task<Response<FineTune>?> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTunes>?> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTune>?> RetrieveFineTuneAsync(string fineTuneId, CancellationToken cancellationToken);

}