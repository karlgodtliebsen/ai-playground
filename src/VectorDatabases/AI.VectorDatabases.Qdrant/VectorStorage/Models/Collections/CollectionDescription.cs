using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Collections;

public class CollectionDescription
{
    [JsonPropertyName("name")] public string Name { get; set; }
}