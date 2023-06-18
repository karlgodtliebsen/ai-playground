using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface IChatCompletionAIClient
{
    Task<Response<ChatCompletions>?> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken);
}