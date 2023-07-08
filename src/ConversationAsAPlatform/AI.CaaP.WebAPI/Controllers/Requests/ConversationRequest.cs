namespace AI.CaaP.WebAPI.Controllers.Requests;

/// <summary>
/// ConversationRequest
/// </summary>
public class ConversationRequest : BaseRequest
{

    /// <summary>
    /// Prompt/Chat text
    /// </summary>
    public TextMessageRequest[] Prompt { get; set; }

    /// <summary>
    /// Continuation of Conversation Id
    /// </summary>
    public Guid? ConversationId { get; set; }

    public bool AddSystemPrompt { get; set; } = true;

    public int SystemPrompt { get; set; } = 1;
}
