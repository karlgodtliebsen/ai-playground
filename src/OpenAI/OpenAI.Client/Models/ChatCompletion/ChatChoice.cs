using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.ChatCompletion;

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



/// </summary>
public class StreamingError
{

    /// <summary>
    /// DeltaIndex.
    /// </summary>
    [JsonPropertyName("error")]
    public ErrorMessage Error { get; set; } = default!;

}

/// </summary>
public class ErrorMessage
{

    /// <summary>
    /// Log Prob Model.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

}
