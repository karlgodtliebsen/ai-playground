using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Search;

public class RangeFilter
{
    [JsonPropertyName("lt")] public object Lt { get; set; }
    [JsonPropertyName("gt")] public object Gt { get; set; }
    [JsonPropertyName("gte")] public object Gte { get; set; }
    [JsonPropertyName("lte")] public object Lte { get; set; }


}