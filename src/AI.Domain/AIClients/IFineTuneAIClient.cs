using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface IFineTuneAIClient
{

    Task<Response<FineTune>?> FineTuneAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTunes>?> GetFineTunesAsync(FineTuneRequest request, CancellationToken cancellationToken);

    Task<Response<FineTune>?> RetrieveFineTuneAsync(string fineTuneId, CancellationToken cancellationToken);

}