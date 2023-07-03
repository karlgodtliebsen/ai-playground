using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Images;

/// <summary>
/// base64 encoded image data
/// </summary>
public class GeneratedImage
{
    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);

    /// <summary>
    /// Image data in base64 encoding.
    /// </summary>
    [JsonPropertyName("data")]
    public ImageData[] Data { get; set; }

}
