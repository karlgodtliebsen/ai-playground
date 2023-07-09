using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Collections;

public class CollectionConfig
{
    [JsonPropertyName("params")] public CollectionParams Params { get; set; }
}