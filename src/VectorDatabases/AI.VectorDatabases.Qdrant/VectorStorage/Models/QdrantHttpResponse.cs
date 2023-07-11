using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models;
/// <summary>
/// Default response from Qdrant
/// </summary>
/// <typeparam name="T"></typeparam>
public class QdrantHttpResponse<T>
{
    [JsonPropertyName("time")] public float Time { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = "ok";

    [JsonPropertyName("result")] public T Result { get; set; } = default!;
}
