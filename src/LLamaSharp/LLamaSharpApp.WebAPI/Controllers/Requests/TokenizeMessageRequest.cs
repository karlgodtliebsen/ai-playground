using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// TokenizeMessageRequest
/// </summary>
public class TokenizeMessageRequest : SimpleTextMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="text"></param>
    public TokenizeMessageRequest(string? text) : base(text) { }
}
