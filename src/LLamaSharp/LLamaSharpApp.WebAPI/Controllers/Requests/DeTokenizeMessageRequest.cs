namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// DeTokenizeMessageRequest
/// </summary>
public class DeTokenizeMessageRequest : BaseRequest
{
    /// <summary>
    /// Array of tokens to be detokenized
    /// </summary>
    public int[] Tokens { get; set; } = default!;

    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = true;
}
