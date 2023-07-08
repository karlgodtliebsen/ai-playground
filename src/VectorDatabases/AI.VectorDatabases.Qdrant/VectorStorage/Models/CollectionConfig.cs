using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models;

public class CollectionConfig
{
    [JsonPropertyName("params")] public CollectionParams Params { get; set; }
}