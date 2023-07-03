using System.Text.Json.Serialization;

using OpenAI.Client.OpenAI.Models.Audio;

namespace OpenAI.Client.OpenAI.Models.Requests;


public class AudioTranscriptionRequest : ModelBaseRequest
{
    public AudioTranscriptionRequest()
    {
        RequestUri = "audio/transcriptions";
        Model = "whisper-1";
    }

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

    [JsonPropertyName("language")]
    public string? Language { get; init; } = default!;

}
