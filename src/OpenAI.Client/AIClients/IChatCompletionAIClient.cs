using OneOf;

using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IChatCompletionAIClient
{
    Task<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken);


    Task<OneOf<ResponseStream<ChatCompletions>, ErrorResponse>> GetChatCompletionsUsingStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

    //Task<OneOf<ChatCompletionsStream, ErrorResponse>> GetChatCompletionsUsingStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

    //    Task<OneOf<TS, ErrorResponse>> GetResponseUsingStreamAsync<TS, TR, T>(T request, CancellationToken cancellationToken)
    //        where TS : ResponseStream<TR>, new()
    //        where TR : class;
}

