namespace AI.CaaP.Domain;

public interface IConversationService
{

    Task<Conversation> RunConversation(ConversationMessage message, CancellationToken cancellationToken);

}


