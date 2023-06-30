namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for SimpleTextMessage
/// </summary>
public class SimpleTextMessage
{
    public SimpleTextMessage(string? text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Text = text;
    }

    public string Text { get; }

    public bool UsePersistedModelState { get; set; } = true;
}
