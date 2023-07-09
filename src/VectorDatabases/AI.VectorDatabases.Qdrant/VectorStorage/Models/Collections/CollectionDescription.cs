using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;

public class CollectionDescription
{
    [JsonPropertyName("name")] public string Name { get; set; }
}