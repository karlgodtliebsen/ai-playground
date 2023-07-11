using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection">Create collection</a>
/// </summary>
public class CreateCollectionBase
{

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("hnsw_config")] public HnswConfig? HnswConfig { get; set; } = default!;
    /// <summary>
    /// Create collection from another collection
    /// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection from another collection</a>
    /// </summary>
    [JsonPropertyName("init_from")] public string? InitFrom { get; set; } = default!;
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("quantization_config")] public string? QuantizationConfig { get; set; } = default!;
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("wal_config")] public WalConfig? WalConfig { get; set; } = default!;
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("optimizers_config")] public OptimizersConfig? OptimizersConfig { get; set; } = default!;

}
