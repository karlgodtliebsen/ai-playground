using AI.CaaP.AgentsDomain;

namespace AI.CaaP.Repositories;

public interface IAgentRepository
{
    Task<List<Agent>> FindAllAgents(CancellationToken cancellationToken);
    Task<Agent?> FindAgent(Guid agentId, CancellationToken cancellationToken);
    Task UpdateAgent(Agent agent, CancellationToken cancellationToken);
    Task<bool> DeleteAgent(Guid agentId, CancellationToken cancellationToken);
    Task<Agent> AddAgent(Agent agent, CancellationToken cancellationToken);
}
