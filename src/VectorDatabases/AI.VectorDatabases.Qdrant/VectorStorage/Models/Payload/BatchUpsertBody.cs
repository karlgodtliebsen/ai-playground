using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// bactch upsert body
/// <a href = "https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class BatchUpsertBody
{
    [JsonPropertyName("batch")]
    public BatchStruct Batch { get; set; }
}
