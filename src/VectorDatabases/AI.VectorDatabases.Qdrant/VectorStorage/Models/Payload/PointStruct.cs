using System.Text.Json.Serialization;

using AI.Library.Utils;

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
    public string Id { get; init; } = default!;

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    //[JsonPropertyName("vector")]
    //public double[] Vector { get; set; } = Array.Empty<double>();

    [JsonPropertyName("vector")]
    [JsonConverter(typeof(ReadOnlyMemoryConverter))]
    public ReadOnlyMemory<float>? Vector { get; init; } = default;

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("payload")]
    public object Payload { get; init; } = default!;
}
