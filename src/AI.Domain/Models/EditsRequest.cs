using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class EditsRequest : BaseRequest
{
    public EditsRequest()
    {
        this.MaxTokens = null;
    }

    [JsonPropertyName("model")]
    public string Model { get; init; }


    [JsonPropertyName("input")]
    public string Input { get; init; } = "";

    [JsonPropertyName("instruction")]
    public string Instruction { get; init; }

}