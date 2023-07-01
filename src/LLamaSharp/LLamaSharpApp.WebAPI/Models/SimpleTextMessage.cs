namespace LLamaSharpApp.WebAPI.Models;

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
        ArgumentNullException.ThrowIfNull(text);
        Text = text;
    }

    /// <summary>
    /// The prompt text
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false;
}
