namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

/// <summary>
/// Request object to hold the text to be sent to the embedding algoritm
/// </summary>
public class EmbeddingsRequest : TextMessageRequest
{
    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false;

}
