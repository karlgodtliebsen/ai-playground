using System.Text.Json.Serialization;
using OpenAI.Client.Domain;

namespace OpenAI.Client.Models.Requests;

public class AudioTranslationRequest : IModelRequest
{

    /// <summary>
    /// Which model was used to generate this result.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = "whisper-1";

    [JsonIgnore]
    public string RequestUri { get; set; } = "audio/translation";

    public string FullFilename { get; set; }

    public string File => Path.GetFileName(FullFilename);

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }
    /// <summary>
    /// The format in which the generated images are returned. Must be one of url or b64_json
    /// Use ImageResponseFormat to set this property
    /// </summary>
    [JsonPropertyName("response_format")]
    public string ResponseFormat => AudioResponseFormat.ToString();


    /// <summary>
    /// The format in which the generated images are returned. Must be one of url or b64_json
    /// </summary>
    [JsonIgnore]
    public AudioResponseFormat AudioResponseFormat { get; init; } = AudioResponseFormat.Json;

    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; } = 1.0;

}