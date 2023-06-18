using AI.Domain.Configuration;
using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

using Microsoft.Extensions.Options;

namespace AI.Domain.AIClients.Implementation;

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