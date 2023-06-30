namespace LLamaSharpApp.WebAPI.Models.Requests;

/// <summary>
/// TokenizeMessageRequest
/// </summary>
public class TokenizeMessageRequest : SimpleTextMessage
{
    public TokenizeMessageRequest(string? text) : base(text) { }
}
