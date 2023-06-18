using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface IEditsAIClient
{
    Task<Response<Completions>?> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken);

}