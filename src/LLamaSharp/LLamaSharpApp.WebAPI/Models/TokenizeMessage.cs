namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for TokenizeMessage
/// </summary>
public class TokenizeMessage : SimpleTextMessage
{
    public TokenizeMessage(string? text) : base(text) { }
}
