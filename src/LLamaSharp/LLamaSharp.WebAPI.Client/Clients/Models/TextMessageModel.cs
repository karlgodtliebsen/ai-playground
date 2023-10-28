namespace LLamaSharp.WebAPI.Client.Clients.Models;

/// <summary>
/// TextMessageRequest
/// </summary>
public class TextMessageModel : BaseModel
{
    /// <summary>
    /// Prompt/Chat text
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// <a href="https://replicate.com/blog/how-to-prompt-llama/">System Prompts</a>
    /// </summary>
    public string? SystemPrompt { get; set; }

}
