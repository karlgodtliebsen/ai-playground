using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

/// <summary>
/// <a href="https://qdrant.tech/documentation/concepts/search/" />
/// </summary>
public class SearchFilter
{

    [JsonPropertyName("should")] public ConditionalFilter[]? Should { get; set; } = default!;
    [JsonPropertyName("must")] public ConditionalFilter[]? Must { get; set; } = default!;
    [JsonPropertyName("must_not")] public ConditionalFilter[]? MustNot { get; set; } = default!;

    public void AddShould(ConditionalFilter filter)
    {
        if (Should == null)
        {
            Should = new ConditionalFilter[] { filter };
        }
        else
        {
            var list = Should.ToList();
            list.Add(filter);
            Should = list.ToArray();
        }
    }

    public void AddMust(ConditionalFilter filter)
    {
        if (Must == null)
        {
            Must = new ConditionalFilter[] { filter };
        }
        else
        {
            var list = Must.ToList();
            list.Add(filter);
            Must = list.ToArray();
        }
    }
    public void AddMustNot(ConditionalFilter filter)
    {
        if (MustNot == null)
        {
            MustNot = new ConditionalFilter[] { filter };
        }
        else
        {
            var list = MustNot.ToList();
            list.Add(filter);
            MustNot = list.ToArray();
        }
    }

}
