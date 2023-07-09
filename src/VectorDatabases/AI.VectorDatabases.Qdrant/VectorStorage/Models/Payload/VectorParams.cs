using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Payload;

public class VectorParams
{
    [JsonPropertyName("size")] public int Size { get; set; }
    [JsonPropertyName("distance")] public string Distance { get; set; }

    [JsonPropertyName("on_disk")] public bool OnDisk { get; set; }

    [JsonPropertyName("hnsw_config")]
    public HnswConfig HnswConfig { get; set; }

    [JsonPropertyName("quantization_config")]
    public ScalarQuantization QuantizationConfig { get; set; }

    [JsonPropertyName("shard_number")] public int? ShardNumber { get; set; } = null;

    [JsonPropertyName("replication_factor")] public int? ReplicationFactor { get; set; } = null;

    [JsonPropertyName("write_consistency_factor")] public int? WriteConsistencyFactor { get; set; } = null;
    [JsonPropertyName("on_disk_payload")] public bool? OnDiskPayload { get; set; } = null;



    public VectorParams(int size = 1, string distance = "Cosine", bool onDisk = true)
    {
        Size = size;
        Distance = distance;
        OnDisk = onDisk;
    }
}
