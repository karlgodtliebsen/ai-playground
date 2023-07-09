using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

public class MatchFilter
{
    [JsonPropertyName("value")] public object Value { get; set; }
}