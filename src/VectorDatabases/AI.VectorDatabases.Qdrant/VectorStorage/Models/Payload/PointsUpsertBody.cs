using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
/// <summary>
/// points
/// <a href = "https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class PointsUpsertBody
{
    [JsonPropertyName("points")]
    public IList<PointStruct> Points { get; set; } = default!;
}
