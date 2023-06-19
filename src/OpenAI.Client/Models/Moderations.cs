using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class ModerationCategories
{
    [JsonPropertyName("hate")]
    public bool Hate { get; set; }
    [JsonPropertyName("hate/threatening")]
    public bool HateThreatening { get; set; }
    [JsonPropertyName("self-harm")]
    public bool SelfHarm { get; set; }
    [JsonPropertyName("sexual")]
    public bool Sexual { get; set; }
    [JsonPropertyName("sexual/minors")]
    public bool SexualMinors { get; set; }
    [JsonPropertyName("violence")]
    public bool Violence { get; set; }
    [JsonPropertyName("violence/graphic")]
    public bool ViolenceGraphic { get; set; }

}


public class ModerationCategoryScores
{
    [JsonPropertyName("hate")]
    public double Hate { get; set; }
    [JsonPropertyName("hate/threatening")]
    public double HateThreatening { get; set; }
    [JsonPropertyName("self-harm")]
    public double SelfHarm { get; set; }
    [JsonPropertyName("sexual")]
    public double Sexual { get; set; }
    [JsonPropertyName("sexual/minors")]
    public double SexualMinors { get; set; }
    [JsonPropertyName("violence")]
    public double Violence { get; set; }
    [JsonPropertyName("violence/graphic")]
    public double ViolenceGraphic { get; set; }
}



public class ModerationResults
{
    [JsonPropertyName("categories")]
    public ModerationCategories Categories { get; set; }
    [JsonPropertyName("category_scores")]
    public ModerationCategoryScores CategoryScores { get; set; }

    [JsonPropertyName("flagged")]
    public ModerationCategoryScores Flagges { get; set; }

}

