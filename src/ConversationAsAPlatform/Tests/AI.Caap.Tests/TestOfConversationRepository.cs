using System.Data.Common;

using AI.CaaP.Configuration;
using AI.CaaP.Domain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.Configuration;
using AI.CaaP.Repository.DatabaseContexts;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Qdrant.Tests.Utils;

using Xunit.Abstractions;

namespace AI.Caap.Tests;

public class TestOfConversationRepository
{
    private readonly ILogger logger;


    private readonly HostApplicationFactory factory;
    public const string IntegrationTests = "integrationtests";
    public const bool UseRelationelDatabase = true;

    public TestOfConversationRepository(ITestOutputHelper output)
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
                if (!UseRelationelDatabase)
                {
                    var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ConversationDbContext>));
                    if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);
                    var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                    if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);
                    services.AddDbContext<ConversationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("ConversationDatabase");
                    });
                }
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();
        if (UseRelationelDatabase)
        {
            this.factory.Services.DestroyMigration();
            this.factory.Services.UseMigration();
        }
    }


    [Fact]
    public async Task PersistAConversation()
    {
        this.factory.Services.CleanDatabase();

        var conversationRepository = factory.Services.GetRequiredService<IConversationRepository>();

        var conversation = new Conversation()
        {
            Content = "content",
            Title = "title",
        };

        await conversationRepository.AddConversation(conversation, CancellationToken.None);
        var persistedConversation = await conversationRepository.GetConversation(conversation.Id, CancellationToken.None);
        persistedConversation.Should().NotBeNull();

        persistedConversation.Should().BeEquivalentTo(conversation, options => options.Excluding(x => x.CreatedTime).Excluding(x => x.UpdatedTime));
    }


}
