using AI.CaaP.Client.Configuration;
using AI.CaaP.Domain;
using AI.CaaP.Repositories;
using AI.CaaP.Repository.Configuration;
using AI.Library.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenAI.Client.Domain;

using OpenAI.Client.OpenAI.Models.ChatCompletion;

const string applicationName = "AI CaaP Client - Conversation as a Platform";

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("To use this application, you must start a Docker instance of Qdrant:");
Console.WriteLine("docker run -p 6333:6333 qdrant/qdrant.");
Console.WriteLine("Or you can connect to a local/cloud service, remember to update appSettings.");

Console.WriteLine();
Console.WriteLine("You must also create a MsSql Database using the Entity Framework Migrations in 'AI.CaaP.Repository' by calling DestroyMigration and UseMigration.");

Console.ResetColor();


Console.WriteLine("Press any key to continue");
Console.ReadLine();


Observability.StartLogging(applicationName);

var builder = Host.CreateApplicationBuilder(args);
builder.WithLogging();
builder.AddSecrets<Program>();
builder.Services
    .AddAppConfiguration(builder.Configuration);



Observability.LogFinalizedConfiguration(applicationName);

IHost host = builder.Build();
using (host)
{
    //Vector database run
    Console.WriteLine("Running a Qdrant VectorDb demo..");
    await RunAVectorDbDemo(host.Services);

    //CaaP run

    Console.WriteLine("Running a Conversation demo..");
    await RunAPersistedConversation(host.Services);

    Console.WriteLine("...");
    Console.ReadLine();
}


Observability.StopLogging(applicationName);


/// <summary>
/// partial program to support testing
/// </summary>
public partial class Program
{
    static async Task RunAVectorDbDemo(IServiceProvider services)
    {
        const string collectionName = "console-app-collection";

        //Vector database run
        Console.WriteLine($"Attempting to Create a Collection {collectionName} in Qdrant");

        var qdrantFactory = services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(4, Distance.DOT, true);
        var client = await qdrantFactory.Create(collectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);
        await client.RemoveCollection(collectionName, CancellationToken.None);

        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            collectionInfo => Console.WriteLine($"Created Collection {collectionName} in Qdrant. Status {collectionInfo}"),
            error => throw new QdrantException(error.Error)
        );
    }

    static async Task RunAPersistedConversation(IServiceProvider services)
    {
        // Short demo. use the AI.CaaP.WebAPI to create a conversation using agents and userId

        //database must be created first
        Console.WriteLine("Recreating the mssql database..");
        services.DestroyMigration();
        services.UseMigration();

        //services.CleanDatabase();
        var conversationRepository = services.GetRequiredService<IConversationRepository>();
        var aiChatService = services.GetRequiredService<IOpenAiChatCompletionService>();

        var agentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        string deploymentName = "gpt-3.5-turbo";

        //create a set of conversation messages
        Console.WriteLine("1: My name is Arthur from the Hitchhikers Guide to the Galaxy");
        var conversation = new Conversation()
        {
            Role = ConversationRole.User.ToRole(),
            Content = "My name is Arthur from the Hitchhikers Guide to the Galaxy",
            AgentId = agentId,
            UserId = userId,
        };
        await conversationRepository.AddConversation(conversation, CancellationToken.None);

        Console.WriteLine("2: What is my name?");
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
                //content.Should().Contain("Arthur Dent");
                Console.WriteLine(content);
            },
            error => throw new AIException(error.Error)
        );
    }

}




