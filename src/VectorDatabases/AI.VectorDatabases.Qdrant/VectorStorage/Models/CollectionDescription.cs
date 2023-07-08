using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;

public class CollectionDescription
{
    [JsonPropertyName("name")] public string Name { get; set; }
}