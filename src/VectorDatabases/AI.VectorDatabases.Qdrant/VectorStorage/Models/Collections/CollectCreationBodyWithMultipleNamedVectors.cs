using System.Text.Json.Serialization;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;

/// <summary>
/// Collection with multiple vectors
/// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection">Create collection</a>
/// </summary>
public class CollectCreationBodyWithMultipleNamedVectors : CreateCollectionBase
{

    [JsonPropertyName("vectors")]
    public IDictionary<string, VectorParams> NamedVectors { get; set; }

}
