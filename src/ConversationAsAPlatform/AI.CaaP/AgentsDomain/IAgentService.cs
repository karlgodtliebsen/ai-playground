namespace AI.CaaP.AgentsDomain;

public interface IAgentService
{
    Task<Agent> CreateAgent(Agent agent, CancellationToken cancellationToken);

    Task<List<Agent>> FindAllAgents(CancellationToken cancellationToken);

    Task<Agent?> FindAgent(Guid userId, CancellationToken cancellationToken);
    Task<bool> DeleteAgent(Guid userId, CancellationToken cancellationToken);
    Task UpdateAgent(Agent agent, CancellationToken cancellationToken);
}
