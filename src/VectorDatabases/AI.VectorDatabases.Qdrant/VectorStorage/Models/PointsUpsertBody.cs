using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;

public class PointsUpsertBody
{
    [JsonPropertyName("points")] public List<PointStruct> Points { get; set; }
}
