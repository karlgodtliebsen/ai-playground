﻿using AI.CaaP.AgentsDomain;
using AI.CaaP.Configuration;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.Configuration;
using AI.Test.Support;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Caap.Tests;

public class TestOfAgentRepository
{
    private readonly ILogger logger;


    private readonly HostApplicationFactory factory;
    public const string IntegrationTests = "integrationtests";

    public TestOfAgentRepository(ITestOutputHelper output)
    {
        this.factory = HostApplicationFactory.Build(
            environment: () => IntegrationTests,
            serviceContext: (services, configuration) =>
            {
                services
                    .AddCaaP(configuration)
                    .AddRepository()
                    .AddDatabaseContext(configuration)
                    ;
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();

        this.factory.Services.DestroyMigration();
        this.factory.Services.UseMigration();
    }


    [Fact]
    public async Task PersistAnAgent()
    {
        this.factory.Services.CleanDatabase();

        var repository = factory.Services.GetRequiredService<IAgentRepository>();
        var conversationId = Guid.NewGuid();
        var agent = new Agent()
        {
            Name = "Arthur",
            Instruction = "Read Hichhiker's Guide to the Galaxy",
            OwnerId = Guid.NewGuid()
        };

        await repository.AddAgent(agent, CancellationToken.None);

        var persistedAgent = await repository.FindAgent(conversationId, agent.Id, CancellationToken.None);
        persistedAgent.Should().NotBeNull();

        persistedAgent.Should().BeEquivalentTo(agent, options => options.Excluding(x => x.CreatedTime).Excluding(x => x.UpdatedTime));
    }

    [Fact]
    public async Task PersistMultipleAgents()
    {
        this.factory.Services.CleanDatabase();

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