using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/points/#delete-payload-keys" />
/// </summary>
public class DeletePointsFilter
{
    /// <summary>
    /// Keys to delete
    /// </summary>
    [JsonPropertyName("points")]
    public IList<string> Ids { get; set; } = new List<string>();


    public DeletePointsFilter()
    {
    }
    public DeletePointsFilter(string id)
    {
        Ids.Add(id);
    }

    public DeletePointsFilter(IEnumerable<string> ids)
    {
        ((List<string>)Ids).AddRange(ids);
    }
}
