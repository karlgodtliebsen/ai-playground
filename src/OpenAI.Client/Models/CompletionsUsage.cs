using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class CompletionsUsage
{
    /// <summary> Number of tokens received in the completion. </summary>
    [JsonPropertyName("completion_tokens")] public int CompletionTokens { get; set; }
    /// <summary> Number of tokens sent in the original request. </summary>
    [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }
    /// <summary> Total number of tokens transacted in this request/response. </summary>
    [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
}