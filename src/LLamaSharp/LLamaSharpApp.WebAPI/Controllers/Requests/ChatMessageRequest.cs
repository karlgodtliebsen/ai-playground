namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// Request object to hold the text/message to be sent to the chatbot
/// </summary>
public class ChatMessageRequest : TextMessageRequest
{
    /// <summary>
    /// When true, the models state will be loaded and saved from the file system
    /// </summary>
    public bool UsePersistedModelState { get; set; }
}
