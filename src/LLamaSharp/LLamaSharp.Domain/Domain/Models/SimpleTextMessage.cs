namespace LLamaSharp.Domain.Domain.Models;

/// <summary>
/// Domain Model for SimpleTextMessage
/// </summary>
public class SimpleTextMessage : BaseMessageModel
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="text"></param>
    public SimpleTextMessage(string? text)
    {
        Text = text;
    }

    /// <summary>
    /// The prompt text
    /// </summary>
    public string? Text { get; }

    /// <summary>
    /// <a href="https://replicate.com/blog/how-to-prompt-llama/">System Prompts</a>
    /// </summary>
    public string? SystemPrompt { get; set; }

    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false;
}
