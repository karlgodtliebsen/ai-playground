using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

public class ChatCompletionAIClient : AIClientBase, IChatCompletionAIClient
{
    public ChatCompletionAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }


    public async Task<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<ChatCompletionRequest, ChatCompletions>(request.RequestUri, request, cancellationToken);
        return result;
    }


    public async Task<OneOf<ResponseStream<ChatCompletions>, ErrorResponse>> GetChatCompletionsUsingStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken)
    {
        return await base.GetResponseUsingStreamAsync<ResponseStream<ChatCompletions>, ChatCompletions, ChatCompletionRequest>(request.RequestUri, request, cancellationToken);
    }

    public async IAsyncEnumerable<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken)
    {
        var collection = base.GetResponseStreamAsync<ChatCompletions, ChatCompletionRequest>(request.RequestUri, request, cancellationToken);
        await foreach (var result in collection.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }
}
