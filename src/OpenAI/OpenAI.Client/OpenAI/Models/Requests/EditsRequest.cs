using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class EditsRequest : BaseRequest
{

    public EditsRequest()
    {
        MaxTokens = null;
        TopP = null;
        Stream = null;
        RequestUri = "edits";

    }

    [JsonPropertyName("input")]
    public string Input { get; init; } = "";


    [JsonPropertyName("instruction")]
    public string Instruction { get; init; }
}
