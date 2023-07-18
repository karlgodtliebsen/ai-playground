using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/points/#delete-payload-keys" />
/// </summary>
public class DeleteFilter
{

    public DeleteFilter()
    {
    }
    public DeleteFilter(string id)
    {
        Keys.Add(id);
    }

    public DeleteFilter(IEnumerable<string> ids)
    {
        ((List<string>)Keys).AddRange(ids);
    }

    /// <summary>
    /// Keys to delete
    /// </summary>
    [JsonPropertyName("keys")]
    public IList<string> Keys { get; set; } = new List<string>();

    /// <summary>
    /// Delete filter
    /// </summary>
    [JsonPropertyName("must")] public ConditionalFilter[]? Must { get; set; } = default!;

}
