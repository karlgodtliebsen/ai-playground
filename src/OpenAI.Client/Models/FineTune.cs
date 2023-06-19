using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class FineTune
{

    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "fine-tune";


    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("created_at")]
    public long Created { get; set; }
    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);


    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("updated_at")]
    public long Updated { get; set; }

    public DateTimeOffset UpdatedDate => DateTimeOffset.FromUnixTimeSeconds(Updated);


    /// <summary>
    /// Which model was used to generate this result.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("organization_id")]
    public string? OrganizationId { get; set; } = default!;

    [JsonPropertyName("result_files")]
    public FileData[]? ResultFiles { get; set; } = default!;

    [JsonPropertyName("status")]
    public string? Status { get; set; } = default!;

    [JsonPropertyName("validation_files")]
    public FileData[]? ValidationFiles { get; set; } = default!;

    [JsonPropertyName("events")]
    public FineTuneEvent[]? Events { get; set; } = default!;

    [JsonPropertyName("training_files")]
    public FileData[]? TrainingFiles { get; set; } = default!;

    [JsonPropertyName("hyperparams")]
    public Hyperparams[]? Hyperparams { get; set; } = default!;

}