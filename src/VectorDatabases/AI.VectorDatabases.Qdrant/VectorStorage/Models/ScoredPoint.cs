using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;

public class ScoredPoint
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("version")] public int Version { get; set; }

    [JsonPropertyName("score")] public float Score { get; set; }

    [JsonPropertyName("vector")] public float[] Vector { get; set; }
}