namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// TokenizeMessageRequest
/// </summary>
public class TokenizeMessageRequest : TextMessageRequest
{

    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false;

}
