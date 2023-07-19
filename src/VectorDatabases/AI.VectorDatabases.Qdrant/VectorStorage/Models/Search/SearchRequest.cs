using System.Collections;
using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/points/operation/search_points" />
/// </summary>
public class SearchRequest
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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Vector { get; private set; } = default!;

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
    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SearchFilter? Filter { get; set; } = default!;

    /// <summary>
    /// SearchParams (object) or (any or null)
    /// Additional search params
    /// </summary>
    [JsonPropertyName("params")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Params { get; set; } = default!;

    /// <summary>
    ///   (WithPayloadInterface (WithPayloadInterface (boolean)
    /// or Array of WithPayloadInterface(strings)
    /// or (PayloadSelector (PayloadSelectorInclude (object)
    /// or PayloadSelectorExclude (object)))))
    /// or(any or null)
    /// Select which payload to return with the response.Default: None
    /// </summary>
    [JsonPropertyName("with_payload")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? WithPayload { get; private set; } = default!;

    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    [JsonPropertyName("with_vector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? WithVector { get; private set; } = default!;


    /// <summary>
    /// Read consistency guarantees for the operation
    /// </summary>
    [JsonPropertyName("consistency")]
    public int Consistency { get; set; } = 1;

    /// <summary>
    /// number (float) or null 
    ///  Define a minimal score threshold for the result.
    ///  If defined, less similar results will not be returned.
    ///  Score of the returned result might be higher or smaller than the threshold depending on
    ///  the Distance function used.E.g. for cosine similarity only higher scores will be returned.
    /// </summary>
    [JsonPropertyName("score_threshold")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? ScoreThreshold { get; set; } = default!;

    /// <summary>
    /// Array of vector IDs to retrieve
    /// </summary>
    [JsonPropertyName("ids")]
    public IEnumerable<string> PointIds { get; set; } = new List<string>();


    public SearchRequest WithPointId(string pointId)
    {
        this.PointIds = this.PointIds.Append(pointId);
        return this;
    }

    public SearchRequest WithPointIDs(IEnumerable<string> pointIds)
    {
        this.PointIds = pointIds;
        return this;
    }

    public SearchRequest Take(int count)
    {
        this.Limit = count;
        return this;
    }

    public SearchRequest SimilarToVector(double[] vector)
    {
        Vector = vector;
        return this;
    }

    public SearchRequest SimilarToVector(float[] vector)
    {
        Vector = vector;
        return this;
    }
    public SearchRequest SimilarToVector(IEnumerable<double> vector)
    {
        Vector = vector;
        return this;
    }

    /// <summary>
    /// Named mode:
    /// { "vector": { "vector": [1.0, 2.0, 3.0], "name": "image-embeddings" } }
    /// </summary>
    /// <param name="vector"></param>
    public SearchRequest SimilarToVector(IDictionary vector)
    {
        Vector = vector;
        return this;
    }

    public SearchRequest SimilarToVector(IEnumerable<float> vector)
    {
        this.Vector = vector;
        return this;
    }

    public SearchRequest UseWithPayload(bool withPayload)
    {
        WithPayload = withPayload;
        return this;
    }
    public SearchRequest UseWithPayload(string[] withPayload)
    {
        WithPayload = withPayload;
        return this;
    }

    public SearchRequest IncludePayLoad()
    {
        this.WithPayload = true;
        return this;
    }
    public SearchRequest FromPosition(int offset)
    {
        this.Offset = offset;
        return this;
    }

    public SearchRequest TakeFirst()
    {
        return this.FromPosition(0).Take(1);
    }


    public SearchRequest UseWithPayloadSelectorInclude(string[] withPayload)
    {
        var p = new Dictionary<string, object>();
        var d = new Dictionary<string, object>();
        d.Add("payloadSelectorInclude", withPayload);
        p.Add("payloadSelector", d);
        WithPayload = p;
        return this;
    }

    public SearchRequest UseWithPayloadSelectorExclude(string[] withPayload)
    {
        var p = new Dictionary<string, object>();
        var d = new Dictionary<string, object>();
        d.Add("payloadSelectorExclude", withPayload);
        p.Add("payloadSelector", d);
        WithPayload = p;
        return this;
    }


    public SearchRequest HavingExternalId(string id)
    {
        if (this.Filter == null)
        {
            this.Filter = new SearchFilter();
        }
        this.Filter.AddMustMatch(new ConditionalFilter
        {
            Key = "id",
            Match = new MatchFilter()
            {
                Value = id
            }
        });

        return this;
    }

    public SearchRequest HavingExternalId(IEnumerable<string> ids)
    {
        if (this.Filter == null)
        {
            this.Filter = new SearchFilter();
        }

        foreach (var id in ids)
        {
            this.Filter.AddMustMatch(new ConditionalFilter
            {
                Key = "id",
                Match = new MatchFilter()
                {
                    Value = id
                }
            });
        }
        return this;
    }

    public SearchRequest HavingTags(IEnumerable<string>? tags)
    {
        if (tags == null) { return this; }

        foreach (var tag in tags)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                if (this.Filter == null)
                {
                    this.Filter = new SearchFilter();
                }

                this.Filter.AddMustMatch(new ConditionalFilter
                {
                    Key = "external_tags",
                    Match = new MatchFilter()
                    {
                        Value = tag
                    }
                });
            }
        }

        return this;
    }

    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    public SearchRequest SetWithVector(bool withVector)
    {
        WithVector = withVector;
        return this;
    }
    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    public SearchRequest SetWithVector(string[] withVector)
    {
        WithVector = withVector;
        return this;
    }
    /// <summary>
    /// (WithVector (WithVector (boolean) or Array of WithVector (strings))) or (any or null)
    /// Default: null
    /// Whether to return the point vector with the result?
    /// </summary>
    public SearchRequest SetWithVector(object withVector)
    {
        WithVector = withVector;
        return this;
    }

    public SearchRequest IncludeVectorData(bool withVector)
    {
        this.WithVector = withVector;
        return this;
    }

    public SearchRequest WithScoreThreshold(double threshold)
    {
        this.ScoreThreshold = threshold;
        return this;
    }


}
