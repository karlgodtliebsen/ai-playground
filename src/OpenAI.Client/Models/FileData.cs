using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;


public class FileData
{
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";

    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("bytes")]
    public long Bytes { get; set; }

    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("created_at")]
    public long Created { get; set; }
    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);

    /// <summary> Model used for completion response. </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("purpose")]
    public string Purpose { get; set; }


    /// <summary>
    /// When the object is deleted, this attribute is used in the Delete file operation
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    /// <summary>
    /// The status of the File (ie when an upload operation was done: "uploaded")
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// The status details, it could be null
    /// </summary>
    [JsonPropertyName("status_details")]
    public string StatusDetails { get; set; }

}
