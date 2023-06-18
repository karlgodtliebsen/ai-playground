using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class ImageData
{

    [JsonPropertyName("url")]
    public string Url { get; init; }


    [JsonPropertyName("b64_json")]
    public string Data { get; init; }


}