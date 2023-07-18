using System.Text.Json.Serialization;

using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection">Create collection</a>
/// </summary>
public class CreateCollectionWithVectorRequest : CreateCollectionBase
{
    /// <summary>
    /// Vectors collection name
    /// <a href="https://qdrant.tech/documentation/reference/collections.html#collection-name">Collection name</a>
    /// </summary>
    [JsonPropertyName("vectors")] public VectorParams Vectors { get; init; } = default!;
}

