using AI.ConsoleApp.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using OpenAI.Client.AIClients;
using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;

using System.Diagnostics;

const string applicationName = "AI-playground";

Observability.StartLogging(applicationName);

var builder = Host.CreateApplicationBuilder(args);
builder.AddHostLogging();
builder.AddSecrets<Program>();
builder.Services.AddAppConfiguration(builder.Configuration);

Observability.LogFinalizedConfiguration(applicationName);

IHost host = builder.Build();
using (host)
{

    var aiClient = host.Services.GetRequiredService<IChatCompletionAIClient>()!;
    var options = host.Services.GetRequiredService<IOptions<OpenAIOptions>>()!;
    Debug.Assert(options is not null);
    Debug.Assert(options.Value is not null);
    Debug.Assert(options.Value.ApiKey != "<openai api key>");

    string deploymentName = "gpt-3.5-turbo";
    var messages = new[]
    {
        new ChatCompletionMessage {Role = "system", Content = "You are a helpful assistant.!" },
        new ChatCompletionMessage { Role = "user", Content = "Elaborate on the question: how long until Humanity reach mars?" }
    };
    var payload = new ChatCompletionRequest
    {
        Model = deploymentName,
        Messages = messages
    };

    var charCompletionsResponse = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);

    Debug.Assert(charCompletionsResponse is not null);
    Debug.Assert(charCompletionsResponse.Success);
    Debug.Assert(charCompletionsResponse!.Value is not null);
    foreach (var choice in charCompletionsResponse!.Value!.Choices)
    {
        string completion = choice.Message!.Content!.Trim();
        Console.WriteLine(completion);
    }


    Console.ReadLine();
}


Observability.StopLogging(applicationName);


/// <summary>
/// partial program to support testing
/// </summary>
public partial class Program { }




