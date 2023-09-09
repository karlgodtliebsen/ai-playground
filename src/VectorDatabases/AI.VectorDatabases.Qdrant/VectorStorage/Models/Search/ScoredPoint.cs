using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

public class ScoredPoint
{
    /// <summary>
    /// The unique point id for assigned to the vector index.
    /// </summary>
    [JsonIgnore]
    public string PointId { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("score")]
    public float Score { get; set; }

    [JsonPropertyName("vector")]
    public IEnumerable<float>? Vector { get; set; } = default!;


    public ReadOnlyMemory<float> ToReadOnlyMemoryVector()
    {
        if (Vector is null)
        {
            return ReadOnlyMemory<float>.Empty;
        }
        return new ReadOnlyMemory<float>(Vector.ToArray());
    }


    /// <summary>
    /// The tags used for search.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; }

    [JsonPropertyName("payload")]
    public IDictionary<string, object>? Payload { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(Score)}: {Score}, {nameof(Vector)}: {Vector}, {nameof(Payload)}: {Payload}";
    }
    /// <summary>
    /// Serializes the metadata to JSON.
    /// </summary>
    /// <returns>Serialized payload</returns>
    public string GetSerializedPayload()
    {
        return JsonSerializer.Serialize(this.Payload);
    }

    public void InitializeVectors()
    {
        if (Vector is null)
        {
            this.Vector = Array.Empty<float>();
        }
    }
}
