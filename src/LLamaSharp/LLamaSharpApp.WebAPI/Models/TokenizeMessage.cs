namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for TokenizeMessage
/// </summary>
public class TokenizeMessage : SimpleTextMessage
{
    /// <summary>
    /// Constructor for TokenizeMessage
    /// </summary>
    /// <param name="text"></param>
    public TokenizeMessage(string? text) : base(text) { }
}
