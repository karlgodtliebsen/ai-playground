using AI.CaaP.Domain;

namespace AI.CaaP.Repositories;

public interface IConversationRepository
{
    Task<Conversation> AddConversation(Conversation conversation, CancellationToken cancellationToken);
    Task<Conversation?> GetConversation(Guid conversationId, CancellationToken cancellationToken);

    Task<IList<Conversation>> GetConversationsByUserId(Guid userId, CancellationToken cancellationToken);
}
