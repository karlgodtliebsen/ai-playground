using AI.Caap.Tests.Fixtures;
using AI.CaaP.Domain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.Configuration;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Caap.Tests;

[Collection("Caap Collection")]
public class TestOfConversationRepository
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private const bool UseRelationalDatabase = true;
    private readonly IServiceProvider services;

    public TestOfConversationRepository(ITestOutputHelper output, CaapWithDatabaseTestFixture fixture)
    {
        this.factory = fixture.WithLogging(output).WithDockerSupport().Build();
        this.services = factory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.services.DestroyMigration();
        this.services.UseMigration();
        if (UseRelationalDatabase)
        {
            this.services.DestroyMigration();
            this.services.UseMigration();
        }
    }


    [Fact]
    public async Task PersistAConversation()
    {
        this.services.CleanDatabase();

        var conversationRepository = factory.Services.GetRequiredService<IConversationRepository>();

        var conversation = new Conversation()
        {
            Content = "content",
            Title = "title",
        };

        await conversationRepository.AddConversation(conversation, CancellationToken.None);
        var persistedConversation = await conversationRepository.GetConversationById(conversation.Id, CancellationToken.None);
        persistedConversation.Should().NotBeNull();

        persistedConversation.Should().BeEquivalentTo(conversation, options => options.Excluding(x => x.CreatedTime).Excluding(x => x.UpdatedTime));
    }

    [Fact]
    public async Task RunAConversation()
    {
        this.services.CleanDatabase();
        var conversationRepository = factory.Services.GetRequiredService<IConversationRepository>();
        Guid userId = Guid.NewGuid();
        var conversations = new[]
        {
            new Conversation
            {
                UserId = userId, Role = ConversationRole.System.ToRole(), Content = "You are a helpful assistant."
            },
            new Conversation
            {
                UserId = userId, Role = ConversationRole.User.ToRole(), Content = "Who won the world series in 2020?!"
            },
            new Conversation
            {
                UserId = userId, Role = ConversationRole.Assistant.ToRole(), Content = "The Los Angeles Dodgers won the World Series in 2020."
            },
            new Conversation
            {
                UserId = userId, Role = ConversationRole.User.ToRole(), Content = "Where was it played?"
            },
        };

        await conversationRepository.AddConversation(conversations, CancellationToken.None);
        var persistedConversations = await conversationRepository.GetConversationsByUserId(userId, CancellationToken.None);
        persistedConversations.Count.Should().Be(conversations.Length);
        for (int i = 0; i < persistedConversations.Count; i++)
        {
            var persistedConversation = persistedConversations[i];
            var conversation = conversations[i];
            persistedConversation.Should().BeEquivalentTo(conversation, options => options.Excluding(x => x.CreatedTime).Excluding(x => x.UpdatedTime));
        }
    }
}
