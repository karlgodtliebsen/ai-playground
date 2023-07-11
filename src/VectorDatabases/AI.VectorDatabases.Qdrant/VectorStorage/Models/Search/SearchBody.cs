using System.Collections;
using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/points/operation/search_points" />
/// </summary>
public class SearchBody
{
    /// <summary>
    /// Array of numbers or NamedVector (object) (NamedVectorStruct)
    /// Vector data separator for named and unnamed modes
    /// Unnamed mode:
    /// { "vector": [1.0, 2.0, 3.0]}
    /// Named mode:
    /// { "vector": { "vector": [1.0, 2.0, 3.0], "name": "image-embeddings" } }
    /// </summary>
    [JsonPropertyName("vector")]
    public object Vector { get; private set; } = default!;

    public void SetVector(double[] vector)
    {
        Vector = vector;
    }

    public void SetVector(float[] vector)
    {
        Vector = vector;
    }

    /// <summary>
    /// Named mode:
    /// { "vector": { "vector": [1.0, 2.0, 3.0], "name": "image-embeddings" } }
    /// </summary>
    /// <param name="vector"></param>
    public void SetVector(IDictionary vector)
    {
        Vector = vector;
    }

    /// <summary>
    /// integer <uint> >= 0
    /// Max number of result to return
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Offset of the first result to return.
    /// May be used to paginate results.
    /// Note: large offset values may cause performance issues.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Filter (object) or (any or null)
    /// Look only for points which satisfies this conditions
    /// </summary>
    [JsonPropertyName("filter")] public SearchFilter? Filter { get; set; } = default!;

    /// <summary>
    /// SearchParams (object) or (any or null)
    /// Additional search params
    /// </summary>
    [JsonPropertyName("params")] public object? Params { get; set; } = default!;

    /// <summary>
    ///   (WithPayloadInterface (WithPayloadInterface (boolean)
    /// or Array of WithPayloadInterface(strings)
    /// or (PayloadSelector (PayloadSelectorInclude (object)
    /// or PayloadSelectorExclude (object)))))
    /// or(any or null)
    /// Select which payload to return with the response.Default: None
    /// </summary>
    [JsonPropertyName("with_payload")] public object? WithPayload { get; private set; } = default!;


    public void SetWithPayload(bool withPayload)
    {
        WithPayload = withPayload;
    }

    public void SetWithPayload(string[] withPayload)
    {
        WithPayload = withPayload;
    }

    public void SetWithPayloadSelectorInclude(string[] withPayload)
    {
        var p = new Dictionary<string, object>();
        var d = new Dictionary<string, object>();
        d.Add("payloadSelectorInclude", withPayload);
        p.Add("payloadSelector", d);
        WithPayload = p;
    }

    public void SetWithPayloadSelectorExclude(string[] withPayload)
    {
        var p = new Dictionary<string, object>();
        var d = new Dictionary<string, object>();
        d.Add("payloadSelectorExclude", withPayload);
        p.Add("payloadSelector", d);
        WithPayload = p;
    }

    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    [JsonPropertyName("with_vector")] public object? WithVector { get; private set; } = default!;
    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    public void SetWithVector(bool withVector)
    {
        WithVector = withVector;
    }
    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    public void SetWithVector(string[] withVector)
    {
        WithVector = withVector;
    }
    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    public void SetWithVector(object withVector)
    {
        WithVector = withVector;
    }

    /// <summary>
    /// number (float) or null 
    ///  Define a minimal score threshold for the result.
    ///  If defined, less similar results will not be returned.
    ///  Score of the returned result might be higher or smaller than the threshold depending on
    ///  the Distance function used.E.g. for cosine similarity only higher scores will be returned.
    /// </summary>
    [JsonPropertyName("score_threshold")] public float? ScoreThreshold { get; set; } = default!;
}
