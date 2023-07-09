using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Payload;

public class PointsUpsertBody
{
    [JsonPropertyName("points")] public List<PointStruct> Points { get; set; }
}
