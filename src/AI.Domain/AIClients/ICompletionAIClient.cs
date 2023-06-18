using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface ICompletionAIClient
{
    Task<Response<Completions>?> GetCompletionsAsync(CompletionRequest request, CancellationToken none);

}