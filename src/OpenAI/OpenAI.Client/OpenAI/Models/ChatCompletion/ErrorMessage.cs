using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.ChatCompletion;

/// <summary>
/// Error Message.
/// </summary>
public class ErrorMessage
{

    /// <summary>
    /// Log Prob Model.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

}
