using AI.CaaP.AgentsDomain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.DatabaseContexts;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace AI.CaaP.Repository.Repositories;

public class AgentRepository : IAgentRepository
{
    private readonly ConversationDbContext dbContext;

    public AgentRepository(ConversationDbContext dbContext, ILogger logger)
    {
        this.dbContext = dbContext;
    }


    public async Task<List<Agent>> FindAllAgents(CancellationToken cancellationToken)
    {
        var result = dbContext.Agents;
        return await result.ToListAsync(cancellationToken);
    }

    public async Task<Agent?> FindAgent(Guid agentId, Guid userId, CancellationToken cancellationToken)
    {
        var agents = dbContext.Agents.Where(x => x.Id == agentId && x.OwnerId == userId);
        var agent = await agents.FirstOrDefaultAsync(cancellationToken);
        return agent;
    }

    public async Task UpdateAgent(Agent agent, CancellationToken cancellationToken)
    {
        dbContext.Agents.Update(agent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAgent(Guid agentId, CancellationToken cancellationToken)
    {
        var agent = dbContext.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent is null)
        {
            return false;
        }
        dbContext.Agents.Remove(agent);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }


    public async Task<Agent> AddAgent(Agent agent, CancellationToken cancellationToken)
    {
        dbContext.Agents.Add(agent);
        await dbContext.SaveChangesAsync(cancellationToken);
        return agent;
    }

}
