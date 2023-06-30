namespace LLamaSharpApp.WebAPI.Models;

/// <summary>
/// Domain Model for DeTokenizeMessage
/// </summary>
public class DeTokenizeMessage
{
    public DeTokenizeMessage(int[] tokens)
    {
        Tokens = tokens;
    }
    /// <summary>
    /// The tokens to be detokenized
    /// </summary>
    public int[] Tokens { get; }

    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false;
}
