using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

public class PointsUpsertBody
{
    [JsonPropertyName("points")] public List<PointStruct> Points { get; set; }
}
