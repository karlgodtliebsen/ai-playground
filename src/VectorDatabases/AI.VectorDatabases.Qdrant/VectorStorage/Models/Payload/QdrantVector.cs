using System.Text.Json.Serialization;

using AI.Library.Utils;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// Adopted from Microsoft Semantic Kernel sdk
/// A record structure used by Qdrant that contains an embedding and metadata.
/// </summary>
public class QdrantVector
{
    /// <summary>
    /// The unique point id for assigned to the vector index.
    /// </summary>
    [JsonIgnore]
    public string PointId { get; }

    /// <summary>
    /// The embedding data.
    /// </summary>
    [JsonPropertyName("vectors")]
    [JsonConverter(typeof(ReadOnlyMemoryConverter))]
    public ReadOnlyMemory<float>? Vectors { get; }

    /// <summary>
    /// The metadata.
    /// </summary>
    [JsonPropertyName("payload")]
    public Dictionary<string, object> Payload { get; }

    /// <summary>
    /// The tags used for search.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; }

    /// <summary>
    /// Constructor for a QdrantVector.
    /// </summary>
    /// <param name="pointId"></param>
    /// <param name="vectors"></param>
    /// <param name="payload"></param>
    /// <param name="tags"></param>
    public QdrantVector(string pointId, IEnumerable<float> vectors, Dictionary<string, object> payload, List<string>? tags = null)
    {
        this.PointId = pointId;
        this.Vectors = new ReadOnlyMemory<float>(vectors.ToArray());
        this.Payload = payload;
        this.Tags = tags;
    }
}
