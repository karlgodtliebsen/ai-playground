using System.Data.Common;

using AI.CaaP.Configuration;
using AI.CaaP.Domain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.Configuration;
using AI.CaaP.Repository.DatabaseContexts;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;

using Qdrant.Tests.Utils;

using Xunit.Abstractions;

namespace AI.Caap.Tests;

public class TestOfConversation
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;


    private readonly HostApplicationFactory factory;
    public const string IntegrationTests = "integrationtests";

    private readonly IModelRequestFactory requestFactory;


    public TestOfConversation(ITestOutputHelper output)
    {
        this.output = output;
        this.factory = HostApplicationFactory.Build(
            environment: () => IntegrationTests,
            serviceContext: (services, configuration) =>
            {
                services
                    .AddCaaP(configuration)
                    .AddOpenAIConfiguration(configuration)
                    .AddRepository()
                    .AddDatabaseContext(configuration)
                    ;

                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ConversationDbContext>));
                if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);
                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);
                services.AddDbContext<ConversationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("Conversations");
                });

            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();
        requestFactory = factory.Services.GetRequiredService<IModelRequestFactory>();
    }


    //https://github.com/Azure-Samples/cosmosdb-chatgpt/blob/main/Services/ChatService.cs
    //_maxConversationTokens = Int32.TryParse(maxConversationTokens, out _maxConversationTokens) ? _maxConversationTokens : 4000;

    //Create conversational agents
    //	https://api.openai.com/v1/chat/completions

    [Fact]
    public async Task RunAConversation()
    {
        var aiChatClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();
        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage { Role = ConversationRole.System.ToRole(), Content = "You are a helpful assistant." },
            new ChatCompletionMessage { Role = ConversationRole.User.ToRole(), Content = "Who won the world series in 2020?!" },
            new ChatCompletionMessage { Role = ConversationRole.Assistant.ToRole(), Content = "The Los Angeles Dodgers won the World Series in 2020." },
            new ChatCompletionMessage { Role = ConversationRole.User.ToRole(), Content = "Where was it played?" },
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages,
                Temperature = 0.0
            });

        var response = await aiChatClient.GetChatCompletionsAsync(payload, CancellationToken.None);
        response.Switch(
            completions =>
            {
                completions.Choices.Count.Should().Be(1);
                string completion = completions.Choices.First().Message!.Content!.Trim();
                completion.Should().Contain("The 2020 World Series was played at Globe Life Field in Arlington, Texas");
                output.WriteLine(completion);
            },
            error => throw new AIException(error.Error)
        );
    }


    [Fact]
    public async Task RunAConversationUsingANameQuestion()
    {

        var aiChatService = factory.Services.GetRequiredService<IOpenAiChatCompletionService>();
        var agentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        string deploymentName = "gpt-3.5-turbo";

        ////a set of conversation messages
        var conversation = new Conversation()
        {
            Role = ConversationRole.User.ToRole(),
            Content = "My name is Arthur",
            AgentId = agentId,
            UserId = userId,
        };

        var conversation2 = new Conversation()
        {
            Role = ConversationRole.User.ToRole(),
            Content = "What is my name?",
            AgentId = agentId,
            UserId = userId,
        };

        var messages = new[]
        {
            new ChatCompletionMessage { Role = conversation.Role, Content = conversation.Content },
            new ChatCompletionMessage { Role = conversation2.Role, Content = conversation2.Content },
        };

        var response = await aiChatService.GetChatCompletion(messages.ToList(), conversation.UserId, deploymentName, CancellationToken.None);
        response.Switch(
            r =>
            {
                var content = r.response.Message!.Content;
                content.Should().Contain("Arthur");
                output.WriteLine(content);
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task RunAPersistedConversation()
    {
        this.factory.Services.CleanDatabase();
        var conversationRepository = factory.Services.GetRequiredService<IConversationRepository>();
        var aiChatService = factory.Services.GetRequiredService<IOpenAiChatCompletionService>();

        var agentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        string deploymentName = "gpt-3.5-turbo";

        //create a set of conversation messages
        var conversation = new Conversation()
        {
            Role = ConversationRole.User.ToRole(),
            Content = "My name is Arthur",
            AgentId = agentId,
            UserId = userId,
        };
        await conversationRepository.AddConversation(conversation, CancellationToken.None);
        conversation = new Conversation()
        {
            Role = ConversationRole.User.ToRole(),
            Content = "What is my name?",
            AgentId = agentId,
            UserId = userId,
        };
        await conversationRepository.AddConversation(conversation, CancellationToken.None);

        var messages = new List<ChatCompletionMessage>();
        var allConversations = await conversationRepository.GetConversationsByUserId(userId, CancellationToken.None);
        foreach (var conv in allConversations)
        {
            messages.Add(new ChatCompletionMessage { Role = conv.Role, Content = conv.Content });
        }


        var response = await aiChatService.GetChatCompletion(messages.ToList(), conversation.UserId, deploymentName, CancellationToken.None);
        response.Switch(
            r =>
            {
                var content = r.response.Message!.Content;
                content.Should().Contain("Arthur");
                output.WriteLine(content);
            },
            error => throw new AIException(error.Error)
        );
    }
}
