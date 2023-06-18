using AI.ConsoleApp.Configuration;
using AI.Domain.AIClients;
using AI.Domain.Configuration;
using AI.Domain.Models;
using AI.Domain.Models.Requests;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System.Diagnostics;

const string applicationName = "AI-playground";

ObservabilityConfigurator.StartLogging(applicationName);

var builder = Host.CreateApplicationBuilder(args);
builder.AddHostLogging();
builder.AddSecrets<Program>();
builder.Services.AddAppConfiguration(builder.Configuration);

ObservabilityConfigurator.LogFinalizedConfiguration(applicationName);

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
        new ChatCompletionMessage { Role = "user", Content = "How long until we reach mars?" }
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

    string completion = charCompletionsResponse!.Value!.Choices[0].Message.Content.Trim();
    Console.WriteLine(completion);
    Console.ReadLine();
}


ObservabilityConfigurator.StopLogging(applicationName);


/// <summary>
/// partial program to support testing
/// </summary>
public partial class Program { }




