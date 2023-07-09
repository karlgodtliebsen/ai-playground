using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Collections;

public class CollectionList
{
    [JsonPropertyName("collections")] public List<CollectionDescription> Collections { get; set; } = new List<CollectionDescription>();
}