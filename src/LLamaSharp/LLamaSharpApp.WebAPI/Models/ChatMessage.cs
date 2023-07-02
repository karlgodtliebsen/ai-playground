namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for ChatMessage
/// </summary>
public class ChatMessage : SimpleTextMessage
{
    /// <summary>
    /// Constructor for ChatMessage
    /// </summary>
    /// <param name="text"></param>
    public ChatMessage(string? text) : base(text) { }

    /// <summary>
    /// The user id
    /// </summary>
    public string UserId { get; set; } = default!;

}
