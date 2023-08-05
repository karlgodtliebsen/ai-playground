namespace LLamaSharpApp.WebAPI.Domain.Models;

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
    /// The user id. Obtained from Security Infrastructure
    /// </summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Use the systems build in AntiPrompt like  [ "User:" ]
    /// </summary>
    public bool UseDefaultAntiPrompt { get; init; } = false;

    /// <summary>
    /// Use the systems build in Prompt
    /// </summary>
    public bool UseDefaultPrompt { get; init; } = false;
}
