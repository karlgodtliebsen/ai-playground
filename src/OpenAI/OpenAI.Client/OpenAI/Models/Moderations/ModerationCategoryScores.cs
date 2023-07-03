using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Moderations;

public class ModerationCategoryScores
{
    [JsonPropertyName("hate")]
    public float Hate { get; set; }
    [JsonPropertyName("hate/threatening")]
    public float HateThreatening { get; set; }
    [JsonPropertyName("self-harm")]
    public float SelfHarm { get; set; }
    [JsonPropertyName("sexual")]
    public float Sexual { get; set; }
    [JsonPropertyName("sexual/minors")]
    public float SexualMinors { get; set; }
    [JsonPropertyName("violence")]
    public float Violence { get; set; }
    [JsonPropertyName("violence/graphic")]
    public float ViolenceGraphic { get; set; }


    [JsonPropertyName("self-harm/intent")]
    public float SelfHarmIntent { get; set; }

    [JsonPropertyName("self-harm/instruction")]
    public float SelfHarmInstruction { get; set; }


    [JsonPropertyName("harassment")]
    public float Harassment { get; set; }


    [JsonPropertyName("harassment/threatening")]
    public float HarassmentThreatening { get; set; }


}
