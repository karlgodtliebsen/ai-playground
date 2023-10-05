using Microsoft.Extensions.Hosting;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

//https://github.com/confluentinc/confluent-kafka-dotnet/
//https://github.com/confluentinc/WikiEdits/blob/master/Program.cs

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder.Configuration);

//builder.Services.AddKafkaProducerHosts(builder.Configuration);
//builder.Services.AddKafkaConsumerHosts(builder.Configuration);
builder.Services.AddKafkaStreamingHosts(builder.Configuration);


var host = builder.Build();
using (host)
{
    Console.WriteLine("Press any key to exit");
    await host.RunAsync();
    Console.ReadLine();
}

namespace SemanticMemory.Kafka.StreamingNewsFeed
{
    /// <summary>
    /// Partial part of Program to support web application factory during test
    /// </summary>
// ReSharper disable once UnusedType.Global
    public partial class Program
    {
    }
}
