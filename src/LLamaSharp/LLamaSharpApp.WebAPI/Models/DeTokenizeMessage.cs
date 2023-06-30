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
    public int[] Tokens { get; }
    public bool UsePersistedModelState { get; set; } = true;
}
