using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

/// <summary>
/// 
/// </summary>
public class GeneratedImage
{
    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);

    [JsonPropertyName("data")]
    public ImageData[] Data { get; set; }

}