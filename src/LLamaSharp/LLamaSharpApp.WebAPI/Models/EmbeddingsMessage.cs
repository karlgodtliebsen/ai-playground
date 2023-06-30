namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain object to hold the text to be sent to the embedding algoritm
/// </summary>
public class EmbeddingsMessage : SimpleTextMessage
{
    /// <summary>
    /// Constructor for EmbeddingsMessage
    /// </summary>
    /// <param name="text"></param>
    public EmbeddingsMessage(string? text) : base(text) { }
}
