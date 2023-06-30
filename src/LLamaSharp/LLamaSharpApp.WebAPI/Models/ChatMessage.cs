namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for ChatMessage
/// </summary>
public class ChatMessage : SimpleTextMessage
{
    public ChatMessage(string? text) : base(text) { }
}
