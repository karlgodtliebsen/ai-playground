namespace AI.CaaP.Domain;

public partial class AgentService : IAgentService
{

    //private readonly IUserIdentity _user;

    public AgentService( /*IUserIdentity user*/)
    {
        //_user = user;
    }

    public string GetAgentDataDir(string agentId)
    {
        var dir = Path.Combine("data", agentId);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }
    public async Task<List<Agent>> GetAgents()
    {
        throw new NotImplementedException();
        //var db = _services.GetRequiredService<AgentDbContext>();
        //var query = from agent in db.Agent
        //    where agent.OwnerId == _user.Id
        //    select agent.ToAgent();
        //return query.ToList();
    }

    public async Task<Agent> GetAgent(string id)
    {
        throw new NotImplementedException();
        //var db = _services.GetRequiredService<AgentDbContext>();
        //var query = from agent in db.Agent
        //    where agent.Id == id
        //    select agent.ToAgent();

        //var profile = query.FirstOrDefault();
        //var dir = GetAgentDataDir(id);

        //var instructionFile = Path.Combine(dir, "instruction.txt");
        //if (File.Exists(instructionFile))
        //{
        //    profile.Instruction = File.ReadAllText(instructionFile);
        //}

        //var samplesFile = Path.Combine(dir, "samples.txt");
        //if (File.Exists(samplesFile))
        //{
        //    profile.Samples = File.ReadAllText(Path.Combine(dir, "samples.txt"));
        //}

        //return profile;
    }

    public async Task UpdateAgent(Agent agent)
    {
        throw new NotImplementedException();
        //var db = _services.GetRequiredService<AgentDbContext>();

        //db.Transaction<IAgentTable>(delegate
        //{
        //    var record = db.Agent.FirstOrDefault(x => x.OwnerId == agent.OwerId && x.Id == agent.Id);

        //    record.Name = agent.Name;
        //    record.Description = agent.Description;
        //    record.UpdatedDateTime = DateTime.UtcNow;
        //});

        //// Save instruction to file
        //var dir = GetAgentDataDir(agent.Id);
        //var instructionFile = Path.Combine(dir, "instruction.txt");
        //File.WriteAllText(instructionFile, agent.Instruction);

        //var samplesFile = Path.Combine(dir, "samples.txt");
        //File.WriteAllText(samplesFile, agent.Samples);
    }

    public async Task<bool> DeleteAgent(string id)
    {
        throw new NotImplementedException();
    }


    public async Task<Agent> CreateAgent(Agent agent)
    {
        throw new NotImplementedException();

        //var db = _services.GetRequiredService<AgentDbContext>();
        //var record = db.Agent.FirstOrDefault(x => x.OwnerId == _user.Id && x.Name == agent.Name);
        //if (record != null)
        //{
        //    return record.ToAgent();
        //}

        //record = AgentRecord.FromAgent(agent);
        //record.Id = Guid.NewGuid().ToString();
        //record.OwnerId = _user.Id;
        //record.CreatedDateTime = DateTime.UtcNow;
        //record.UpdatedDateTime = DateTime.UtcNow;

        //db.Transaction<IAgentTable>(delegate
        //{
        //    db.Add<IAgentTable>(record);
        //});

        //return record.ToAgent();
    }
}
