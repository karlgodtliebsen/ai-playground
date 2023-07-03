using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class EditsRequest : ModelBaseRequest
{

    public EditsRequest()
    {
        RequestUri = "edits";
    }

    [JsonPropertyName("input")]
    public string Input { get; init; } = "";


    [JsonPropertyName("instruction")]
    public string Instruction { get; init; }
}
