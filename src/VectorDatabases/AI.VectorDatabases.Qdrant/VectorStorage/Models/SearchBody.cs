using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;

public class SearchBody
{
    [JsonPropertyName("vector")] public float[] Vector { get; set; }

    [JsonPropertyName("limit")] public int Limit { get; set; }

    [JsonPropertyName("offset")] public int Offset { get; set; }

    [JsonPropertyName("score_threshold")] public float ScoreThreshold { get; set; }
}