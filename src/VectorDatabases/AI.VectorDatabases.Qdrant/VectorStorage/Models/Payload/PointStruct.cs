using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class PointStruct
{
    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("id")]
    public object Id { get; init; } = default!;

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("vector")]
    public double[] Vector { get; set; } = Array.Empty<double>();


    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("payload")]
    public object? Payload { get; init; } = default!;
}
