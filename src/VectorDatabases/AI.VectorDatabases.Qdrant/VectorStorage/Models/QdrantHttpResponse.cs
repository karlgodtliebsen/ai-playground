using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models;

public class QdrantHttpResponse<T>
{
    [JsonPropertyName("time")] public float Time { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = "ok";

    [JsonPropertyName("result")] public T Result { get; set; }
}