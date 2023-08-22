using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

public class QdrantHttpResponse<T>
{
    [JsonPropertyName("time")] public float Time { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = "ok";

    [JsonPropertyName("result")] public T Result { get; set; } = default!;
}