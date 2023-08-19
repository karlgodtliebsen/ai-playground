using System.Diagnostics.CodeAnalysis;

using AI.CaaP.AgentsDomain;
using AI.CaaP.Domain;

using Microsoft.EntityFrameworkCore;

namespace AI.CaaP.Repository.DatabaseContexts;

[ExcludeFromCodeCoverage]
public class ConversationDbContext : DbContext
{
    public DbSet<Conversation> Conversations { get; set; } = null!;

    public DbSet<Agent> Agents { get; set; } = null!;


    public ConversationDbContext(DbContextOptions<ConversationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }

    public void Migrate()
    {
        this.Database.Migrate();
    }

    public void Clean()
    {
        Conversations.RemoveRange(Conversations);
        Agents.RemoveRange(Agents);
        this.SaveChanges(true);
    }
}
