using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// Batch upsert body
/// <a href = "https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class BatchUpsertBody
{
    [JsonPropertyName("batch")]
    public BatchStruct Batch { get; private set; }

    public BatchUpsertBody(BatchStruct batch)
    {
        this.Batch = batch;
    }

    [JsonIgnore]
    public int Dimension
    {
        get
        {
            if (Batch.Vectors.Length == 0)
            {
                return 0;
            }
            return Batch.Vectors[0].Length;
        }
    }
}

