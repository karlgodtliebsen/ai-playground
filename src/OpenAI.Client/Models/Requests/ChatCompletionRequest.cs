using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Requests;

public class ChatCompletionRequest : BaseRequest
{
    public ChatCompletionRequest()
    {
        base.RequestUri = "chat/completions";
    }

    [JsonPropertyName("messages")]
    public ChatCompletionMessage[] Messages { get; init; }

    [JsonPropertyName("functions")]
    public Functions[]? Functions { get; init; } = default!;


    [JsonPropertyName("function_call")]
    public object? FunctionCall { get; init; } = default!;

}