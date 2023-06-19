using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;


public class Files
{
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("data")]
    public FileData[] FileData { get; set; }

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";

}