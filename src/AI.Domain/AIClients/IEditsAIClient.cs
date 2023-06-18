using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface IEditsAIClient
{
    Task<Response<Completions>?> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken);

}