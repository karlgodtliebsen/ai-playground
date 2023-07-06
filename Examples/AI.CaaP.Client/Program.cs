using AI.CaaP.Client.Configuration;
using AI.Library.Configuration;
using AI.Library.Qdrant.VectorStorage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string applicationName = "AI CaaP Client - Conversation as a Platform";
const string collectionName = "console-app-collection";

Console.WriteLine("To use this application, you must start a Docker instance of Qdrant, or use local/cloud service. Remember to update appSettings ");


Observability.StartLogging(applicationName);

var builder = Host.CreateApplicationBuilder(args);
builder.AddHostLogging();
builder.AddSecrets<Program>();
builder.Services.AddAppConfiguration(builder.Configuration);

Observability.LogFinalizedConfiguration(applicationName);

IHost host = builder.Build();
using (host)
{

    Console.WriteLine($"Attempting to Created a Collection {collectionName} in Qdrant");

    var client = host.Services.GetRequiredService<IVectorDb>();

    var vectorParams = client.CreateParams(4, "Dot", true);
    var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
    result.Switch(

        collectionInfo => Console.WriteLine($"Created Collection {collectionName} in Qdrant. Status {collectionInfo.Status}"),
        error => throw new QdrantException(error.Error)
    );


    //IAgentService

    //var aiClient = host.Services.GetRequiredService<IChatCompletionAIClient>()!;
    //var options = host.Services.GetRequiredService<IOptions<OpenAIOptions>>()!;
    //var requestFactory = host.Services.GetRequiredService<IModelRequestFactory>();
    //Debug.Assert(options is not null);
    //Debug.Assert(options.Value is not null);
    //Debug.Assert(options.Value.ApiKey != "<openai api key>");

    //string deploymentName = "gpt-3.5-turbo";
    //var messages = new[]
    //{
    //    new ChatCompletionMessage {Role = "system", Content = "You are a helpful assistant.!" },
    //    new ChatCompletionMessage { Role = "user", Content = "Elaborate on the question: how long until Humanity reach the Planet Mars?" }
    //};

    //var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
    //    new ChatCompletionRequest
    //    {
    //        Model = deploymentName,
    //        Messages = messages
    //    });
    //var charCompletionsResponse = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
    //Debug.Assert(charCompletionsResponse.IsT0);
    //Debug.Assert(charCompletionsResponse!.AsT0 is not null);
    //foreach (var choice in charCompletionsResponse!.AsT0!.Choices)
    //{
    //    string completion = choice.Message!.Content!.Trim();
    //    Console.WriteLine(completion);
    //}

    Console.WriteLine("...");
    Console.ReadLine();
}


Observability.StopLogging(applicationName);


/// <summary>
/// partial program to support testing
/// </summary>
public partial class Program { }




