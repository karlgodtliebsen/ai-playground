using AI.CaaP.AgentsDomain;

namespace AI.CaaP.Domain;

public interface IConversationService
{

    Task<Conversation> ExecuteConversation(Guid userId, Agent agent, IList<Conversation> conversations, CancellationToken cancellationToken);

}
