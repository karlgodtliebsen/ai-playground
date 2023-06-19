using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

/// <summary>
/// Represents a language model
/// </summary>
public class Model
{
    /// <summary>
    /// The id/name of the model
    /// </summary>
    [JsonPropertyName("id")]
    public string ModelID { get; set; }

    /// <summary>
    /// The owner of this model.  Generally "openai" is a generic OpenAI model, or the organization if a custom or finetuned model.
    /// </summary>
    [JsonPropertyName("owned_by")]
    public string OwnedBy { get; set; }

    /// <summary>
    /// The type of object. Should always be 'model'.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; }

    /// The time when the model was created
    public DateTime? CreatedAt => Created.HasValue ? (DateTime?)(DateTimeOffset.FromUnixTimeSeconds(Created.Value).DateTime) : null;

    /// <summary>
    /// The time when the model was created in unix epoch format
    /// </summary>
    [JsonPropertyName("created")]
    public long? Created { get; set; }

    /// <summary>
    /// Permissions for use of the model
    /// </summary>
    [JsonPropertyName("permission")]
    public List<Permissions> Permission { get; set; } = new List<Permissions>();



}