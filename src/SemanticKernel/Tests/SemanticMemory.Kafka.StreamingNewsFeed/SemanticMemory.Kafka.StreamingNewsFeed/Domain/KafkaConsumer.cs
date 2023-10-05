using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

public class KafkaConsumer
{
    private readonly KafkaConfiguration config;

    public KafkaConsumer(IOptions<KafkaConfiguration> options)
    {
        config = options.Value;
    }


    // Consume messages from the specified Kafka topic.
    public void Consume(string topicName)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        Console.WriteLine($"{nameof(Consume)} starting");

        // Configure the consumer group based on the provided configuration. 
        var consumerConfig = new ConsumerConfig(config);
        consumerConfig.GroupId = "wiki-edit-stream-group-1";

        // The offset to start reading from if there are no committed offsets
        // (or there was an error in retrieving offsets).
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;

        // Do not commit offsets.
        consumerConfig.EnableAutoCommit = false;

        // Enable canceling the consumer loop with Ctrl+C.

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
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
                var cr = consumer.Consume(cts.Token);

                // Parse the JSON to extract the URI of the edited page.
                var jsonDoc = JsonDocument.Parse(cr.Message.Value);

                // For consuming from the recent_changes topic. 
                var metaElement = jsonDoc.RootElement.GetProperty("meta");
                var uriElement = metaElement.GetProperty("uri");
                var uri = uriElement.GetString();

                // For consuming from the ksqlDB sink topic.
                // var editsElement = jsonDoc.RootElement.GetProperty("NUM_EDITS");
                // var edits = editsElement.GetInt32();
                // var uri = $"{cr.Message.Key}, edits = {edits}";

                Console.WriteLine($"Consumed record with URI {uri}");
            }
        }
        catch (OperationCanceledException)
        {
            // Ctrl+C was pressed.
            Console.WriteLine($"Ctrl+C pressed, consumer exiting");
        }
        finally
        {
            consumer.Close();
        }
    }

}