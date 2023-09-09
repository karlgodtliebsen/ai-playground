using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

/// <summary>
/// <a href = "https://qdrant.tech/documentation/concepts/points/" >Qdrant Points</a>>
/// </summary>
public class BatchRequestStruct
{
    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("ids")]
    public IList<string> Ids { get; set; } = new List<string>();

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("vectors")]
    public IList<IEnumerable<double>> Vectors { get; set; } = new List<IEnumerable<double>>();


    public void AddToVectors(ReadOnlyMemory<float> vector)
    {
        List<double> embeddings = new List<double>();
        embeddings.AddRange(vector.ToArray().Select(v => (double)v).ToArray());
        Vectors.Add(embeddings);
    }


    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("payloads")]
    public IList<Dictionary<string, object>> Payloads { get; set; } = new List<Dictionary<string, object>>();

    public BatchRequestStruct(IList<double[]> vectors, IList<string> ids, IList<Dictionary<string, object>> payloads)
    {
        this.UpsertRange(vectors, ids, payloads);
    }
    public BatchRequestStruct(double[][] vectors, string[] ids, IList<Dictionary<string, object>> payloads)
    {
        this.UpsertRange(vectors, ids, payloads);
    }
    public BatchRequestStruct()
    {
    }

    /// <summary>
    /// Upsert range
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="ids"></param>
    /// <param name="payloads"></param>
    public void UpsertRange(double[][] vectors, string[] ids, IList<Dictionary<string, object>> payloads)
    {
        for (int i = 0; i < vectors.Length; i++)
        {
            this.Vectors.Add(vectors[i]);
            this.Ids.Add(ids[i]);
            this.Payloads.Add(payloads[i]);
        }
    }

    /// <summary>
    /// Upsert range
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="ids"></param>
    /// <param name="payloads"></param>
    public void UpsertRange(IList<double[]> vectors, IList<string> ids, IList<Dictionary<string, object>> payloads)
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            this.Vectors.Add(vectors[i]);
            this.Ids.Add(ids[i]);
            this.Payloads.Add(payloads[i]);
        }
    }
}
