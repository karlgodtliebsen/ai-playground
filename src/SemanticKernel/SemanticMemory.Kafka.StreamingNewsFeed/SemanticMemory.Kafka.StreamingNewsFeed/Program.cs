using AI.Library.Configuration;

using Microsoft.Extensions.Hosting;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

using Serilog;

//https://github.com/confluentinc/confluent-kafka-dotnet/
//https://github.com/confluentinc/WikiEdits/blob/master/Program.cs
const string applicationName = "Wiki-media-stream-To-SemanticMemory";

Observability.UseBootstrapLogger(applicationName);


var builder = Host.CreateApplicationBuilder(args);
builder.WithLogging();
builder.AddSecrets<Program>();
builder.Services.AddDomain(builder.Configuration);

Observability.LogFinalizedConfiguration(applicationName);

var host = builder.Build();
using (host)
{
    Observability.LogFinalizedConfiguration(applicationName);
    Log.Logger.Information("Running...");
    await host.RunAsync();
    Console.ReadLine();
}

Observability.StopLogging(applicationName);

/// <summary>
/// Partial part of Program to support web application factory during test
/// </summary>
// ReSharper disable once UnusedType.Global
public partial class Program
{
}


