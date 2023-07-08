using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;



//https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection

public class CollectCreationBody
{
    [JsonPropertyName("vectors")] public VectorParams Vectors { get; set; }

    [JsonPropertyName("hnsw_config")] public HnswConfig? HnswConfig { get; set; } = null;

    [JsonPropertyName("init_from")] public string? InitFrom { get; set; } = null;
    [JsonPropertyName("quantization_config")] public string? QuantizationConfig { get; set; } = null;
    [JsonPropertyName("wal_config")] public WalConfig? WalConfig { get; set; } = null;
    [JsonPropertyName("optimizers_config")] public OptimizersConfig? OptimizersConfig { get; set; } = null;

}
