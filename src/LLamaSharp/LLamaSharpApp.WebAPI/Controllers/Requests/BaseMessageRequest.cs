namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// 
/// </summary>
public class BaseMessageRequest
{
    /// <summary>
    /// Prompt/Chat text
    /// </summary>
    public string? Text { get; set; }
}
