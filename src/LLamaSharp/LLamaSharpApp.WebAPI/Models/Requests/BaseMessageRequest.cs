namespace LLamaSharpApp.WebAPI.Models.Requests;

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
