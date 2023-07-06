namespace AI.CaaP.Domain;

public interface IAgentService
{
    Task<Agent> CreateAgent(Agent agent);
    Task<List<Agent>> GetAgents();
    Task<Agent> GetAgent(string id);
    Task<bool> DeleteAgent(string id);
    Task UpdateAgent(Agent agent);
    string GetAgentDataDir(string agentId);
}
