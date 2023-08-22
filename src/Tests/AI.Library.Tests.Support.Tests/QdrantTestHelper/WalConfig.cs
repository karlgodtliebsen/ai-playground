using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

public class WalConfig
{
    [JsonPropertyName("wal_capacity_mb")] public int? WalCapacityMb { get; set; } = null;
    [JsonPropertyName("wal_segments_ahead")] public int? WalSegmentsAhead { get; set; } = null;
}