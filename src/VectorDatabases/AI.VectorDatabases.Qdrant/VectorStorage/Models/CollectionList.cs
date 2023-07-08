using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;

public class CollectionList
{
    [JsonPropertyName("collections")] public List<CollectionDescription> Collections { get; set; } = new List<CollectionDescription>();
}