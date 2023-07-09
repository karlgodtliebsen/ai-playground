using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;

public class CollectionConfig
{
    [JsonPropertyName("params")] public CollectionParams Params { get; set; }
}