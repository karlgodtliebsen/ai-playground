using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class PointStructWithNamedVector
{
    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("id")]
    public object Id { get; init; } = default!;

    /// <summary>
    /// vector with named dimensions
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("vector")]
    public Dictionary<string, double[]> Vector { get; set; } = default!;


    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("payload")]
    public object? Payload { get; init; } = default!;
}
