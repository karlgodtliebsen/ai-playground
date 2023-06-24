using System.Text.Json.Serialization;

using OpenAI.Client.Models.ChatCompletion;

namespace OpenAI.Client.Models.Chat;

public class ChatChoice
{

    /// <summary> Index. </summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; } = default!;

    /// <summary> Log Prob Model. </summary>
    [JsonPropertyName("message")]
    public ChatCompletionMessage? Message { get; set; } = default!;

    /// <summary> Reason for finishing. </summary>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; } = default!;
}
