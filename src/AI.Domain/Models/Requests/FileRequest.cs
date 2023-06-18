using System.Text.Json.Serialization;

namespace AI.Domain.Models.Requests;

public class FileRequest
{
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("data")]
    public FileData[] FileData { get; set; }

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";


    [JsonPropertyName("purpose")]
    public string Purpose { get; set; }

}