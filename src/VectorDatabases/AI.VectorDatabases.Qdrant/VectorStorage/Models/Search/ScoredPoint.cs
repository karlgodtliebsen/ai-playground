using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

public class ScoredPoint
{
    [JsonPropertyName("id")] public object Id { get; set; } = default!;

    [JsonPropertyName("version")] public int Version { get; set; }

    [JsonPropertyName("score")] public float Score { get; set; }

    [JsonPropertyName("vector")] public float[]? Vector { get; set; }

    [JsonPropertyName("payload")] public object? Payload { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(Score)}: {Score}, {nameof(Vector)}: {Vector}, {nameof(Payload)}: {Payload}";
    }
}
