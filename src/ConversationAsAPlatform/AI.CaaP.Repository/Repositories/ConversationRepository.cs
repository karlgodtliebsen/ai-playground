using AI.CaaP.Domain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.DatabaseContexts;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace AI.CaaP.Repository.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ConversationDbContext dbContext;

    public ConversationRepository(ConversationDbContext dbContext, ILogger logger)
    {
        this.dbContext = dbContext;
    }
    public async Task<Conversation> AddConversation(Conversation conversation, CancellationToken cancellationToken)
    {
        await dbContext.Conversations.AddAsync(conversation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    public async Task<IList<Conversation>> AddConversation(IList<Conversation> conversations, CancellationToken cancellationToken)
    {
        await dbContext.Conversations.AddRangeAsync(conversations, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return conversations;
    }

    public async Task<Conversation?> GetConversationById(long conversationId, CancellationToken cancellationToken)
    {
        return await dbContext.Conversations.FindAsync(conversationId, cancellationToken);
    }

    public async Task<IList<Conversation>> GetConversationsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var result = dbContext.Conversations.Where(x => x.UserId == userId).OrderBy(x => x.Id);
        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IList<Conversation>> GetConversationsByAgentId(Guid agentId, CancellationToken cancellationToken)
    {
        var result = dbContext.Conversations.Where(x => x.AgentId == agentId).OrderBy(x => x.Id);
        return await result.ToListAsync(cancellationToken);
    }
}
