using OneOf;

using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IChatCompletionAIClient
{
    Task<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken);


    Task<OneOf<ResponseStream<ChatCompletions>, ErrorResponse>> GetChatCompletionsUsingStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

    IAsyncEnumerable<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

}

