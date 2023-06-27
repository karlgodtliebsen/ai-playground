using System.Text.Json.Serialization;

using OpenAI.Client.Models.ChatCompletion;

namespace OpenAI.Client.Models.Embeddings;

public class Embeddings
{
    /// <summary>
    /// Id for completion response.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Object for completion response.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";

    /// <summary>
    /// Created time for completion response.
    /// </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }

    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);


    /// <summary>
    /// Model used for completion response.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Array of choices returned containing text completions to prompts sent.
    /// </summary>
    [JsonPropertyName("data")]
    public IReadOnlyList<EmbeddingsData> Data { get; set; }

    /// <summary>
    /// Usage counts for tokens input using the completions API.
    /// </summary>
    [JsonPropertyName("usage")]
    public CompletionsUsage Usage { get; set; }
}
