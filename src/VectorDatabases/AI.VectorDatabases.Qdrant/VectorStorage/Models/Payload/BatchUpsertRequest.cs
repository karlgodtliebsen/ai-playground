using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// Batch upsert body
/// <a href = "https://qdrant.tech/documentation/concepts/points/" />
/// </summary>
public class BatchUpsertRequest
{
    [JsonPropertyName("batch")]
    public BatchRequestStruct BatchRequest { get; private set; }

    public BatchUpsertRequest(BatchRequestStruct batchRequest)
    {
        this.BatchRequest = batchRequest;
    }

    public void UpsertRange(double[][] vectors, string[] ids, IList<Dictionary<string, object>> payloads)
    {
        BatchRequest.UpsertRange(vectors, ids, payloads);
    }

    public void UpsertRange(IList<double[]> vectors, IList<string> ids, IList<Dictionary<string, object>> payloads)
    {
        BatchRequest.UpsertRange(vectors, ids, payloads);
    }

    [JsonIgnore]
    public int Dimension
    {
        get
        {
            if (BatchRequest.Vectors.Count == 0)
            {
                return 0;
            }
            return BatchRequest.Vectors[0].Count();
        }
    }
}
