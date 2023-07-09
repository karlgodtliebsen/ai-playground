using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.ChatCompletion;

/// </summary>
public class StreamingError
{

    /// <summary>
    /// DeltaIndex.
    /// </summary>
    [JsonPropertyName("error")]
    public ErrorMessage Error { get; set; } = default!;

}
