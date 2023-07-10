using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// <a href = "https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class BatchStruct
{
    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("ids")]
    public object[] Ids { get; init; } = default!;

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("vectors")]
    public double[][] Vectors { get; set; } = default!;

    [JsonPropertyName("payloads")]
    public object? Payloads { get; init; } = default!;
}
