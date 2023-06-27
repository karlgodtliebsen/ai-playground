using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.ChatCompletion;

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
