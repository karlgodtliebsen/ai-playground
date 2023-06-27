using System.Text.Json.Serialization;

using OpenAI.Client.Models.Files;

namespace OpenAI.Client.Models.Requests;

public class FileRequest
{
    /// <summary>
    /// Id for completion response.
    /// </summary>
    [JsonPropertyName("data")]
    public FileData[] FileData { get; set; }

    /// <summary>
    /// Object for completion response.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";

    /// <summary>
    /// The purpose of the file
    /// </summary>
    [JsonPropertyName("purpose")]
    public string Purpose { get; set; }

}
