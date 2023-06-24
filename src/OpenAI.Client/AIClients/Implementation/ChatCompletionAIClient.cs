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
        return await base.GetResponseUsingStreamAsync<ResponseStream<ChatCompletions>, ChatCompletions, ChatCompletionRequest>("chat/completions", request, cancellationToken);
        //var result = await PostAsyncWithStream<ChatCompletionRequest/*T*/, ResponseStream<ChatCompletions>>(request.RequestUri, request, cancellationToken);
        //return result.Match<OneOf<ResponseStream<ChatCompletions>/*TS*/, ErrorResponse>>(
        //    success =>
        //    {
        //        var data = success.Split("data:");
        //        var resp = new ResponseStream<ChatCompletions>();//TS
        //        foreach (var v in data.Where(d => !string.IsNullOrEmpty(d) && !d.Contains("[DONE]")))
        //        {
        //            var obj = JsonSerializer.Deserialize<ChatCompletions>(v);//T
        //            resp.Data.Add(obj!);
        //        }
        //        return resp;
        //    },
        //    error => new ErrorResponse(error.Error)
        //);
    }

}
