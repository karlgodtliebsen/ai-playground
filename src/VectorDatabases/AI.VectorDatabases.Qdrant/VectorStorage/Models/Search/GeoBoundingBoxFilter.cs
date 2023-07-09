using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

public class GeoBoundingBoxFilter
{
    [JsonPropertyName("top_left")] public TBRFilter? TopLeft { get; set; } = default!;

    [JsonPropertyName("bottom_right")] public TBRFilter? BottomRight { get; set; } = default!;
}