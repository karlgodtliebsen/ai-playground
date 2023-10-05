using System.Text.Json;

using Confluent.Kafka;

using Microsoft.Extensions.Options;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

public class KafkaConsumer
{
    private readonly IEventsToSemanticMemory handleEvents;
    private readonly ILogger logger;
    private readonly KafkaConfiguration config;

    public KafkaConsumer(IEventsToSemanticMemory handleEvents, IOptions<KafkaConfiguration> options, ILogger logger)
    {
        this.handleEvents = handleEvents;
        this.logger = logger;
        config = options.Value;
    }

    // Consume messages from the specified Kafka topic.
    public async Task Consume(string topicName, CancellationToken cancellationToken)
    {
        logger.Information("{name} starting", nameof(Consume));

        // Configure the consumer group based on the provided configuration. 
        var consumerConfig = new ConsumerConfig(config)
        {
            GroupId = config.GroupId,
            // The offset to start reading from if there are no committed offsets
            // (or there was an error in retrieving offsets).
            AutoOffsetReset = AutoOffsetReset.Earliest,
            // Commit offsets.
            EnableAutoCommit = true
        };
        // Build a consumer that uses the provided configuration.
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        // Subscribe to events from the topic.
        consumer.Subscribe(topicName);

        try
        {
            // Run until the terminal receives Ctrl+C. 
            while (true)
            {
                // Consume and deserialize the next message.
                var cr = consumer.Consume(cancellationToken);

                // Parse the JSON to extract the URI of the edited page.
                var jsonDoc = JsonDocument.Parse(cr.Message.Value);

                // For consuming from the recent_changes topic. 
                var metaElement = jsonDoc.RootElement.GetProperty("meta");
                var uriElement = metaElement.GetProperty("uri");
                var uri = uriElement.GetString();
                var key = cr.Message.Key;
                var value = cr.Message.Value;
                await handleEvents.AddMessage(key, value, cancellationToken);
                logger.Information("Consumed record with URI {uri}", uri);
            }
        }
        catch (Exception ex)
        {
            logger.Information(ex, "Consumer Error");
        }
        finally
        {
            consumer.Close();
        }
    }

}
