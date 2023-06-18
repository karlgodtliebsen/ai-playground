using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class Choice
{

    /// <summary> Generated text for given completion prompt. </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = default!;

    /// <summary> Index. </summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; } = default!;

    /// <summary> Log Prob Model. </summary>
    [JsonPropertyName("logprobs")]
    public CompletionsLogProbability? Logprobs { get; set; } = default!;

    /// <summary> Reason for finishing. </summary>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; } = default!;
}