using AI.CaaP.Domain;

namespace AI.CaaP.WebAPI.Controllers.Requests;

/// <summary>
/// ConversationResponse
/// </summary>
public class ConversationResponse
{

    /// <summary>
    /// The Conversation Reply
    /// </summary>
    public Conversation Conversation { get; set; }
}
