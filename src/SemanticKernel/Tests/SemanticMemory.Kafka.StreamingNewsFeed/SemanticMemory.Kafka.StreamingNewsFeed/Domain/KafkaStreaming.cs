using Confluent.Kafka;

using Microsoft.Extensions.Options;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Metrics;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

/// <summary>
/// using.Net Streaming library from LGouellec:
/// https://github.com/LGouellec/kafka-streams-dotnet
/// https://lgouellec.github.io/kafka-streams-dotnet/
/// References
/// https://github.com/confluentinc/confluent-kafka-dotnet/issues/344
/// https://github.com/confluentinc/confluent-kafka-dotnet/issues/1266
/// https://github.com/mhowlett/howlett-kafka-extensions
/// </summary>
public class KafkaStreaming
{
    private readonly KafkaConfiguration configuration;
    private readonly StreamConfig<StringSerDes, StringSerDes> streamConfig;
    public KafkaStreaming(IOptions<KafkaConfiguration> options)
    {
        configuration = options.Value;
        streamConfig = CreateConfig(configuration);
    }

    private StreamConfig<StringSerDes, StringSerDes> CreateConfig(KafkaConfiguration cfg)
    {
        var config = new StreamConfig<StringSerDes, StringSerDes>
        {
            ApplicationId = cfg.ApplicationId,
            BootstrapServers = cfg.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            StateDir = Path.Combine("."),
            CommitIntervalMs = 5000,
            Guarantee = ProcessingGuarantee.AT_LEAST_ONCE,
            MetricsRecording = MetricsRecordingLevel.DEBUG
        };

        return config;
    }

    public async Task UseStreams(string topicSource, string topicDestination)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var builder = new StreamBuilder();

        //more samples at
        //https://github.com/LGouellec/kafka-streams-dotnet/blob/develop/samples/sample-stream-demo/Program.cs
        //https://github.com/LGouellec/kafka-streams-dotnet/blob/develop/samples/sample-stream-registry/Program.cs

        builder.Stream<string, string>(topicSource)
            //  .Filter((k, v) => v.Contains("https://www.wikidata.org/wiki/Q87358856"))
            .To(topicDestination);

        Topology topology = builder.Build();

        var stream = new KafkaStream(topology, streamConfig);

        Console.CancelKeyPress += (o, e) =>
        {
            stream.Dispose();
        };

        await stream.StartAsync();
    }

}
