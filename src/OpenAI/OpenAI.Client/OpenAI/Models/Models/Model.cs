using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Models;

/// <summary>
/// Represents a language model
/// </summary>
public class Model
{
    /// <summary>
    /// The id/name of the model
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    /// <summary>
    /// The owner of this model.  Generally "openai" is a generic OpenAI model, or the organization if a custom or finetuned model.
    /// </summary>
    [JsonPropertyName("owned_by")]
    public string OwnedBy { get; init; } = default!;

    /// <summary>
    /// The type of object. Should always be 'model'.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = default!;

    /// The time when the model was created
    public DateTime? CreatedAt => Created.HasValue ? DateTimeOffset.FromUnixTimeSeconds(Created.Value).DateTime : null;

    /// <summary>
    /// The time when the model was created in unix epoch format
    /// </summary>
    [JsonPropertyName("created")]
    public long? Created { get; init; } = default!;

    /// <summary>
    /// Permissions for use of the model
    /// </summary>
    [JsonPropertyName("permission")]
    public List<Permissions> Permission { get; set; } = new List<Permissions>();

    public Model(string name)
    {
        Id = name;
    }

    /// <summary>
    /// Represents a generic Model/model
    /// </summary>
    public Model()
    {
    }
}
