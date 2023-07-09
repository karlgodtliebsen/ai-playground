using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Search;

public class ConditionalFilter
{
    [JsonPropertyName("key")] public string Key { get; set; }
    [JsonPropertyName("match")] public MatchFilter Match { get; set; }
    [JsonPropertyName("range")] public RangeFilter Range { get; set; } = default!;
    [JsonPropertyName("geo_bounding_box")] public GeoBoundingBoxFilter GeoBoundingBox { get; set; } = default!;
    [JsonPropertyName("geo_radius")] public GeoRadiusFilter GeoRadius { get; set; } = default!;
    [JsonPropertyName("values_count")] public RangeFilter? ValuesCount { get; set; } = default!;
}