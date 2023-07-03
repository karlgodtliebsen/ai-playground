using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Requests;

/// <summary>
/// https://platform.openai.com/docs/api-reference/moderations/create
/// </summary>
public class ModerationRequest : ModelBaseRequest
{
    public ModerationRequest()
    {
        RequestUri = "moderations";
        Model = "text-moderation-stable";
    }

    [JsonPropertyName("input")]
    public string Input { get; init; } = "";
}
