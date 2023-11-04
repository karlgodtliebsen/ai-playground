using AI.Library.Configuration;

using Kernel.Memory.NewsFeed.Domain.Domain.Implementation;
using Kernel.Memory.NewsFeed.Producer.Host.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

//https://github.com/confluentinc/confluent-kafka-dotnet/
//https://github.com/confluentinc/WikiEdits/blob/master/Program.cs
const string applicationName = "Wiki-media-stream-Producer";

Observability.UseBootstrapLogger(applicationName);

var builder = Host.CreateApplicationBuilder(args)
    .WithLogging()
    .AddSecrets<Program>();
builder.Services.AddHosting(builder.Configuration);

Observability.LogFinalizedConfiguration(applicationName);

var host = builder.Build();
using (host)
{
    var client = host.Services.GetRequiredService<KafkaAdminClient>();
    await client.CreateTopic(CancellationToken.None);

    Observability.LogFinalizedConfiguration(applicationName);
    Log.Logger.Information("Running...");

    await host.RunAsync();

}

Observability.StopLogging(applicationName);

/// <summary>
/// Partial part of Program to support web application factory during test
/// </summary>
// ReSharper disable once UnusedType.Global
public partial class Program
{
}





