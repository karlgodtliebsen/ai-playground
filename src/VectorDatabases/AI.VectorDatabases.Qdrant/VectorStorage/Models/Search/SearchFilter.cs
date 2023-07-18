using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/search/" />
/// </summary>
public class SearchFilter
{

    [JsonPropertyName("should")] public IList<ConditionalFilter>? Should { get; set; } = default!;
    [JsonPropertyName("must")] public IList<ConditionalFilter>? Must { get; set; } = default!;
    [JsonPropertyName("must_not")] public IList<ConditionalFilter>? MustNot { get; set; } = default!;

    public void AddShouldMatch(ConditionalFilter filter)
    {
        if (Should == null)
        {
            Should = new List<ConditionalFilter>() { filter };
        }
        else
        {
            Should.Add(filter);
        }
    }

    public void AddMustMatch(ConditionalFilter filter)
    {
        if (Must == null)
        {
            Must = new List<ConditionalFilter>() { filter };
        }
        else
        {
            Must.Add(filter);
        }
    }

    public void AddMustNotMatch(ConditionalFilter filter)
    {
        if (MustNot == null)
        {
            MustNot = new List<ConditionalFilter>() { filter };
        }
        else
        {
            MustNot.Add(filter);
        }
    }
}
