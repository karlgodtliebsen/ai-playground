using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class ChatCompletions
{
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "chat.completion";

    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }

    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);


    /// <summary> Model used for completion response. </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary> Array of choices returned containing text completions to prompts sent. </summary>
    [JsonPropertyName("choices")]
    public IReadOnlyList<ChatChoice> Choices { get; set; }

    /// <summary> Usage counts for tokens input using the completions API. </summary>
    [JsonPropertyName("usage")]
    public CompletionsUsage Usage { get; set; }
}