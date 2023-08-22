using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

public class CollectionConfig
{
    [JsonPropertyName("params")] public CollectionParams Params { get; set; }
}