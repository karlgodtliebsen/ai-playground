using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Moderations;

public class ModerationResults
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("categories")]
    public ModerationCategories Categories { get; set; }

    [JsonPropertyName("category_scores")]
    public ModerationCategoryScores CategoryScores { get; set; }

    [JsonPropertyName("flagged")]
    public ModerationCategoryScores Flagged { get; set; }

}

