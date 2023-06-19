using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class Audio
{
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
}