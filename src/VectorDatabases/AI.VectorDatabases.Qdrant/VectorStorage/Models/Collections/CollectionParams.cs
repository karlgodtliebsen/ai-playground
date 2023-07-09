using System.Text.Json.Serialization;
using AI.VectorDatabaseQdrant.VectorStorage.Models.Payload;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Collections;

public class CollectionParams
{
    [JsonPropertyName("shard_number")] public int ShardNumber { get; set; }

    [JsonPropertyName("replication_factor")] public int ReplicationFactor { get; set; }

    [JsonPropertyName("write_consistency_factor")] public int WriteConsistencyFactor { get; set; }

    [JsonPropertyName("on_disk_payload")] public bool OnDiskPayload { get; set; }

    [JsonPropertyName("vectors")] public VectorParams Vectors { get; set; }
}