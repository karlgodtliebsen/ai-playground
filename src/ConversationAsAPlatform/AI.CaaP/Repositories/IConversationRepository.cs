using AI.CaaP.Domain;

namespace AI.CaaP.Repositories;

public interface IConversationRepository
{
    Task<Conversation> AddConversation(Conversation conversation, CancellationToken cancellationToken);

    Task<IList<Conversation>> AddConversation(IList<Conversation> conversations, CancellationToken cancellationToken);

    Task<Conversation?> GetConversationById(long conversationId, CancellationToken cancellationToken);

    Task<IList<Conversation>> GetConversationsByUserId(Guid userId, CancellationToken cancellationToken);


    Task<IList<Conversation>> GetConversationsByAgentId(Guid agentId, CancellationToken cancellationToken);
}
