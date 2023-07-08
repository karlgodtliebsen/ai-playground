using AI.CaaP.Repositories;

namespace AI.CaaP.AgentsDomain;

public class AgentService : IAgentService
{
    private readonly IAgentRepository repository;

    public AgentService(IAgentRepository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Agent>> FindAllAgents(CancellationToken cancellationToken)
    {
        return await repository.FindAllAgents(cancellationToken);
    }

    public async Task<Agent?> FindAgent(Guid userId, CancellationToken cancellationToken)
    {
        return await repository.FindAgent(userId, cancellationToken);
    }

    public async Task UpdateAgent(Agent agent, CancellationToken cancellationToken)
    {
        await repository.UpdateAgent(agent, cancellationToken);
    }

    public async Task<bool> DeleteAgent(Guid userId, CancellationToken cancellationToken)
    {
        return await repository.DeleteAgent(userId, cancellationToken);
    }

    public async Task<Agent> CreateAgent(Agent agent, CancellationToken cancellationToken)
    {
        return await repository.AddAgent(agent, cancellationToken);
    }
}
