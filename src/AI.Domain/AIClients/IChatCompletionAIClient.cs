using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface IChatCompletionAIClient
{

    Task<Response<ChatCompletions>?> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken);


}