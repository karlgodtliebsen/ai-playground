using System.Runtime.CompilerServices;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class ChatCompletionAIClient : AIClientBase, IChatCompletionAIClient
{
    public ChatCompletionAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIConfiguration> options,
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
        return await GetResponseUsingStreamAsync<ResponseStream<ChatCompletions>, ChatCompletions, ChatCompletionRequest>(request.RequestUri, request, cancellationToken);
    }

    public async IAsyncEnumerable<OneOf<ChatCompletions, ErrorResponse>> GetChatCompletionsStreamAsync(ChatCompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var collection = GetResponseStreamAsync<ChatCompletions, ChatCompletionRequest>(request.RequestUri, request, cancellationToken);
        await foreach (var result in collection.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }
}
