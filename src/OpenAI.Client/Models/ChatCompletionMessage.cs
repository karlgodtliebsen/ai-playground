using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class ChatCompletionMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; init; } = default!;

    [JsonPropertyName("content")]
    public string? Content { get; init; } = default!;

}