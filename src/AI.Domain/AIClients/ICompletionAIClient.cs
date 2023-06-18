using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface ICompletionAIClient
{
    Task<Response<Completions>?> GetCompletionsAsync(CompletionRequest request, CancellationToken none);

}