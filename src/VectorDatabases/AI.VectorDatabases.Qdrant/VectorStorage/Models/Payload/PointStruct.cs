using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Payload;

public class PointStruct
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("vector")] public float[] Vector { get; set; }


    [JsonPropertyName("payload")]

    public object Payload { get; set; }
}
