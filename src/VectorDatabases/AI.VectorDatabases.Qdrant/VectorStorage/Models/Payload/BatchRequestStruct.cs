using System.Text.Json.Serialization;

using AI.Library.Utils;

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
    //[JsonPropertyName("vectors")]
    //public IList<IEnumerable<float>> Vectors { get; set; } = new List<IEnumerable<float>>();

    //public void AddToVectors(ReadOnlyMemory<float> vector)
    //{
    //    Vectors.Add(vector.ToArray());
    //}


    [JsonPropertyName("vectors")]
    [JsonConverter(typeof(ReadOnlyMemoryConverterArray))]
    public ReadOnlyMemory<float>[] Vectors
    {
        get
        {
            return embeddings!.ToArray();
        }
        set
        {
            embeddings = new List<ReadOnlyMemory<float>>(value);
        }
    }

    private IList<ReadOnlyMemory<float>> embeddings { get; set; } = new List<ReadOnlyMemory<float>>();
    public void AddToVectors(ReadOnlyMemory<float> vector)
    {
        embeddings.Add(vector);
    }

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/points/" />
    /// </summary>
    [JsonPropertyName("payloads")]
    public IList<Dictionary<string, object>> Payloads { get; set; } = new List<Dictionary<string, object>>();

    public BatchRequestStruct(IList<ReadOnlyMemory<float>> vectors, IList<string> ids, IList<Dictionary<string, object>> payloads)
    {
        this.UpsertRange(vectors, ids, payloads);
    }
    public BatchRequestStruct(ReadOnlyMemory<float>[] vectors, string[] ids, IList<Dictionary<string, object>> payloads)
    {
        this.UpsertRange(vectors, ids, payloads);
    }
    public BatchRequestStruct()
    {
    }

    ///// <summary>
    ///// Upsert range
    ///// </summary>
    ///// <param name="vectors"></param>
    ///// <param name="ids"></param>
    ///// <param name="payloads"></param>
    //public void UpsertRange(float[][] vectors, string[] ids, IList<Dictionary<string, object>> payloads)
    //{
    //    for (int i = 0; i < vectors.Length; i++)
    //    {
    //        this.Vectors.Add(new ReadOnlyMemory<float>(vectors[i]));
    //        this.Ids.Add(ids[i]);
    //        this.Payloads.Add(payloads[i]);
    //    }
    //}

    /// <summary>
    /// Upsert range
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="ids"></param>
    /// <param name="payloads"></param>
    public void UpsertRange(IList<float[]> vectors, IList<string> ids, IList<Dictionary<string, object>> payloads)
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            this.AddToVectors(new ReadOnlyMemory<float>(vectors[i]));
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
    public void UpsertRange(IList<ReadOnlyMemory<float>> vectors, IList<string> ids, IList<Dictionary<string, object>> payloads)
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            this.AddToVectors(vectors[i]);
            this.Ids.Add(ids[i]);
            this.Payloads.Add(payloads[i]);
        }
    }
}

