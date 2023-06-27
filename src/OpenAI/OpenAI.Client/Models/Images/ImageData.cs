using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.Images;

/// <summary>
/// Support for Url or base64 encoded image data
/// </summary>
public class ImageData
{

    [JsonPropertyName("url")]
    public string? Url { get; init; }


    [JsonPropertyName("b64_json")]
    public string? Data { get; init; }

}
