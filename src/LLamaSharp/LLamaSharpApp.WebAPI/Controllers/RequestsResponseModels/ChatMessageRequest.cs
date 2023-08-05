namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

/// <summary>
/// Request object to hold the text/message to be sent to the chat bot
/// </summary>
public class ChatMessageRequest : TextMessageRequest
{
    /// <summary>
    /// When true, the models state will be loaded and saved from storage
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false!;

    /// <summary>
    /// Use the systems build in AntiPrompt
    /// </summary>
    public bool UseDefaultAntiPrompt { get; set; } = false!;

    /// <summary>
    /// Use the systems build in Prompt
    /// </summary>
    public bool UseDefaultPrompt { get; set; } = false;

}
