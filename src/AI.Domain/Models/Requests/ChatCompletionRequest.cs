using System.Text.Json.Serialization;

namespace AI.Domain.Models.Requests;

public class ChatCompletionRequest : BaseRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; }


    [JsonPropertyName("messages")]
    public ChatCompletionMessage[] Messages { get; init; }

    [JsonPropertyName("functions")]
    public Functions[]? Functions { get; init; } = default!;


    [JsonPropertyName("function_call")]
    public object? FunctionCall { get; init; } = default!;

}