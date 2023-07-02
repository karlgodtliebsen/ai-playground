namespace LLamaSharpApp.WebAPI.Domain.Models;

/// <summary>
/// Domain Model for DeTokenizeMessage
/// </summary>
public class DeTokenizeMessage : BaseMessageModel
{
    /// <summary>
    /// Constructor for DeTokenizeMessage
    /// </summary>
    /// <param name="tokens"></param>
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

    /// <summary>
    /// The user id
    /// </summary>
    public string UserId { get; set; } = default!;
}
