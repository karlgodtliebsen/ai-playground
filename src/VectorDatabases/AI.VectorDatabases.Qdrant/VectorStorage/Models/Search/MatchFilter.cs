using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Search;

public class MatchFilter
{
    [JsonPropertyName("value")] public object Value { get; set; }
}