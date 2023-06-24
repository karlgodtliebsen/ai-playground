using System.Text.Json.Serialization;

using OpenAI.Client.Models.ChatCompletion;

namespace OpenAI.Client.Models.Chat;

/// <summary>
/// Response model for Chat Completion API.
/// </summary>
public class ChatChoice
{
    /// <summary>
    /// Index.
    /// </summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; } = default!;


    /// <summary>
    /// DeltaIndex.
    /// </summary>
    [JsonPropertyName("delta")]
    public DeltaContent? Delta { get; set; } = default!;


    /// <summary>
    /// Log Prob Model.
    /// </summary>
    [JsonPropertyName("message")]
    public ChatCompletionMessage? Message { get; set; } = default!;

    /// <summary>
    /// Reason for finishing.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; } = default!;
}

/// <summary>
/// Streaming support
/// https://github.com/openai/openai-cookbook/blob/main/examples/How_to_stream_completions.ipynb
/// </summary>
public class DeltaContent
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
    [JsonPropertyName("nothing")]
    public string? Nothing { get; set; }
}
