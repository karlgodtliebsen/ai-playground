using System.Text.Json;

using Confluent.Kafka;

using Kernel.Memory.NewsFeed.Domain.Configuration;

using Microsoft.Extensions.Options;

using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Processors.Public;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;

namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public sealed class KafkaStreamingConsumer : IKafkaConsumer
{
    private readonly IEventsToSemanticMemory handleEvents;
    private readonly ILogger logger;
    private readonly KafkaConfiguration configuration;

    public KafkaStreamingConsumer(IEventsToSemanticMemory handleEvents, IOptions<KafkaConfiguration> options, ILogger logger)
    {
        this.handleEvents = handleEvents;
        this.logger = logger;
        configuration = options.Value;
    }

    public async Task Consume(CancellationToken cancellationToken)
    {
        logger.Information("{name} starting", nameof(Consume));
        var config = new StreamConfig<StringSerDes, StringSerDes>
        {
            ApplicationId = configuration.ApplicationId,
            BootstrapServers = configuration.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        string topicName = configuration.Topic;

        logger.Information("{name} starting", nameof(Process));

        StreamBuilder builder = new StreamBuilder();
        builder.Stream<string, string>(topicName)
            .Process(ProcessorBuilder
                .New<string, string>()
                .Processor(async (record) =>
                {
                    await Process(record, cancellationToken);
                })
                .Build());

        Topology t = builder.Build();
        KafkaStream stream = new KafkaStream(t, config);
        cancellationToken.Register(() => { stream.Dispose(); });

        await stream.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    // Consume messages from the specified Kafka topic.
    private async Task Process(Record<string, string> record, CancellationToken cancellationToken)
    {
        logger.Information("{name} Processing", nameof(Process));

        // Parse the JSON to extract the URI of the edited page.
        var jsonDoc = JsonDocument.Parse(record.Value);
        // For consuming from the recent_changes topic. 
        var metaElement = jsonDoc.RootElement.GetProperty("meta");
        var uriElement = metaElement.GetProperty("uri");
        var uri = uriElement.GetString();

        var metaId = metaElement.GetProperty("id").GetString();
        var key = Guid.NewGuid().ToString();
        if (!string.IsNullOrEmpty(metaId))
        {
            key = metaId;
        }

        var value = record.Value;
        await handleEvents.AddMessage(key, value, cancellationToken);
        logger.Information("Consumed record with URI {uri}", uri);
    }
}
