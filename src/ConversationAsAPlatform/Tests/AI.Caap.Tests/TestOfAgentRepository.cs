using AI.Caap.Tests.Fixtures;
using AI.CaaP.AgentsDomain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.Configuration;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Caap.Tests;

[Collection("Caap Collection")]
public class TestOfAgentRepository
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly IServiceProvider services;

    public TestOfAgentRepository(ITestOutputHelper output, CaapWithDatabaseTestFixture fixture)
    {
        this.factory = fixture.WithLogging(output).WithDockerSupport().Build();
        this.services = factory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.services.DestroyMigration();
        this.services.UseMigration();
    }

    [Fact]
    public async Task PersistAnAgent()
    {
        this.services.CleanDatabase();

        var repository = factory.Services.GetRequiredService<IAgentRepository>();
        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var agent = new Agent()
        {
            Name = "Arthur",
            Instruction = "Read Hichhiker's Guide to the Galaxy",
            OwnerId = userId
        };

        await repository.AddAgent(agent, CancellationToken.None);

        var persistedAgent = await repository.FindAgent(agent.Id, userId, CancellationToken.None);
        persistedAgent.Should().NotBeNull();

        persistedAgent.Should().BeEquivalentTo(agent, options => options.Excluding(x => x.CreatedTime).Excluding(x => x.UpdatedTime));
    }

    [Fact]
    public async Task PersistMultipleAgents()
    {
        this.services.CleanDatabase();

        var repository = factory.Services.GetRequiredService<IAgentRepository>();

        for (int i = 0; i < 10; i++)
        {
            var agent = new Agent()
            {
                Name = "Arthur" + i,
                Instruction = "Read Hichhiker's Guide to the Galaxy",
                Description = "Here is a description",
                Samples = "Some smart sample",
                Knowledges = "Know it all",
                OwnerId = Guid.NewGuid()
            };

            await repository.AddAgent(agent, CancellationToken.None);
        }
        var persistedAgents = await repository.FindAllAgents(CancellationToken.None);
        persistedAgents.Should().NotBeNull();
        persistedAgents.Count.Should().Be(10);
    }
}
