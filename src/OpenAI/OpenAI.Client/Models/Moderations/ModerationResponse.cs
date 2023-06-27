using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Moderations;

public class ModerationResponse
{
    /// <summary>
    /// Id for moderation response.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("bytes")]
    public ModerationResults[] Results { get; set; } = new ModerationResults[0];

}
