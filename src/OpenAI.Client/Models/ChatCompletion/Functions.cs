using System.Text.Json.Serialization;

namespace OpenAI.Client.Models.ChatCompletion;

public class Functions
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("parameters")]
    public object? Parameters { get; init; }

}
