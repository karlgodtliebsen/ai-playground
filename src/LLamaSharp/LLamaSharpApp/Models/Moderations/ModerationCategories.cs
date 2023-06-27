using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Moderations;

public class ModerationCategories
{
    [JsonPropertyName("hate")]
    public bool Hate { get; set; }

    [JsonPropertyName("hate/threatening")]
    public bool HateThreatening { get; set; }

    [JsonPropertyName("self-harm")]
    public bool SelfHarm { get; set; }



    [JsonPropertyName("self-harm/intent")]
    public bool SelfHarmIntent { get; set; }

    [JsonPropertyName("self-harm/instruction")]
    public bool SelfHarmInstruction { get; set; }


    [JsonPropertyName("harassment")]
    public bool Harassment { get; set; }


    [JsonPropertyName("harassment/threatening")]
    public bool HarassmentThreatening { get; set; }


    [JsonPropertyName("sexual")]
    public bool Sexual { get; set; }

    [JsonPropertyName("sexual/minors")]
    public bool SexualMinors { get; set; }

    [JsonPropertyName("violence")]
    public bool Violence { get; set; }

    [JsonPropertyName("violence/graphic")]
    public bool ViolenceGraphic { get; set; }

}
