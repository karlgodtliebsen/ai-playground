namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

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
