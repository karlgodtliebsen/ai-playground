using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Requests;

public class EditsRequest : BaseRequest
{

    public EditsRequest()
    {
        MaxTokens = null;
        base.RequestUri = "edits";
    }


    [JsonPropertyName("input")]
    public string Input { get; init; } = "";


    [JsonPropertyName("instruction")]
    public string Instruction { get; init; }
}