using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;

public class CollectionList
{
    [JsonPropertyName("collections")] public List<CollectionDescription> Collections { get; set; } = new List<CollectionDescription>();
}