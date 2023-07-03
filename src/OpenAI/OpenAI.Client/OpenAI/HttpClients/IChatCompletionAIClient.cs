using OneOf;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IChatCompletionAIClient
{
    Task<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken);


    Task<OneOf<ResponseStream<ChatCompletions>, ErrorResponse>> GetChatCompletionsUsingStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

    IAsyncEnumerable<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

}

