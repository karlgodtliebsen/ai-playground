using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// points with named vectors
/// <a href = "https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class PointsWithNamedVectorsUpsertBody
{
    [JsonPropertyName("points")]
    public IList<PointStructWithNamedVector> Points { get; set; } = default!;
}
