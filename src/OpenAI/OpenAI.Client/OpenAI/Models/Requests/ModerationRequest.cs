using System.Text.Json.Serialization;
using OpenAI.Client.Domain;

namespace OpenAI.Client.OpenAI.Models.Requests;

/// <summary>
/// https://platform.openai.com/docs/api-reference/moderations/create
/// </summary>
public class ModerationRequest : IModelRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "text-moderation-stable";

    [JsonIgnore]
    public string RequestUri { get; set; } = "moderations";


    [JsonPropertyName("input")]
    public string Input { get; init; } = "";


}