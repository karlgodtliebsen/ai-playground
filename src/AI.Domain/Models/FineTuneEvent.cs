using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class FineTuneEvent
{

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "fine-tune-event";

    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("created_at")]
    public long Created { get; set; }
    public DateTimeOffset CreatedDate => DateTimeOffset.FromUnixTimeSeconds(Created);

    [JsonPropertyName("level")]
    public string? Level { get; set; } = default!;

    [JsonPropertyName("message")]
    public string? Message { get; set; } = default!;
}