using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

public class TBRFilter
{
    [JsonPropertyName("lon")] public float Lon { get; set; }
    [JsonPropertyName("lat")] public float Lat { get; set; }
}