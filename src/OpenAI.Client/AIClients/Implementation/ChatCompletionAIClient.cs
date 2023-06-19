using Microsoft.Extensions.Options;
using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
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


    public async Task<Response<ChatCompletions>?> GetChatCompletionsAsync(ChatCompletionRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<ChatCompletionRequest, ChatCompletions>("chat/completions", request, cancellationToken);
        return new Response<ChatCompletions>(result!);
    }


}