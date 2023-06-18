using System.Text.Json.Serialization;

namespace AI.Domain.Models.Requests;

public class EmbeddingsRequest
{

    [JsonPropertyName("model")]
    public string Model { get; init; }


    [JsonPropertyName("input")]
    public string Input { get; init; } = "";

    /// <summary>
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; init; } = default!;

}