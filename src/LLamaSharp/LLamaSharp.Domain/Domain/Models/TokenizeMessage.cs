namespace LLamaSharp.Domain.Domain.Models;

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

    /// <summary>
    /// The user id
    /// </summary>
    public string UserId { get; set; } = default!;
}
