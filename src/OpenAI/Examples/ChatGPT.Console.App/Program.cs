using System.Diagnostics;

using AI.ConsoleApp.Configuration;
using AI.Library.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;

using Serilog;

const string applicationName = "AI-playground";

Observability.UseBootstrapLogger(applicationName);

var builder = Host.CreateApplicationBuilder(args);
builder.WithLogging();
builder.AddSecrets<Program>();
builder.Services.AddAppConfiguration(builder.Configuration);

Observability.LogFinalizedConfiguration(applicationName);

IHost host = builder.Build();
using (host)
{
    var aiClient = host.Services.GetRequiredService<IChatCompletionAIClient>()!;
    var options = host.Services.GetRequiredService<IOptions<OpenAIConfiguration>>()!;
    var requestFactory = host.Services.GetRequiredService<IModelRequestFactory>();
    Debug.Assert(options is not null);
    Debug.Assert(options.Value is not null);
    Debug.Assert(options.Value.ApiKey != "<openai api key>");

    string deploymentName = "gpt-3.5-turbo";
    var messages = new[]
    {
        new ChatCompletionMessage {Role = "system", Content = "You are a helpful assistant.!" },
        new ChatCompletionMessage { Role = "user", Content = "Elaborate on the question: how long until Humanity reach the Planet Mars?" }
    };
    Log.Logger.Information("Sending chat completion request for {DeploymentName} with {@Messages}", deploymentName, messages);
    var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
        new ChatCompletionRequest
        {
            Model = deploymentName,
            Messages = messages
        });
    var charCompletionsResponse = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
    Debug.Assert(charCompletionsResponse.IsT0);
    Debug.Assert(charCompletionsResponse!.AsT0 is not null);
    foreach (var choice in charCompletionsResponse!.AsT0!.Choices)
    {
        string completion = choice.Message!.Content!.Trim();
        Console.WriteLine(completion);
    }

    Console.WriteLine("...");
    Console.ReadLine();
}


Observability.StopLogging(applicationName);


/// <summary>
/// partial program to support testing
/// </summary>
public partial class Program { }




