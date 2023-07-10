using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/points/#delete-payload-keys" />
/// </summary>
public class DeleteFilter
{
    /// <summary>
    /// Keys to delete
    /// </summary>
    [JsonPropertyName("keys")]
    public IList<string> Keys { get; set; }

    /// <summary>
    /// Delete filter
    /// </summary>
    [JsonPropertyName("must")] public ConditionalFilter[]? Must { get; set; } = default!;
}
