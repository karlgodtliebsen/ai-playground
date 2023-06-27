using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Audio;

public class Audio
{
    /// <summary>
    /// Id for Audio response.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
