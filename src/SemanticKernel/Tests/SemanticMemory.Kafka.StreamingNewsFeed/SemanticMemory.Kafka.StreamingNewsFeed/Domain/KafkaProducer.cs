using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

public class KafkaProducer
{
    private readonly KafkaConfiguration config;

    public KafkaProducer(IOptions<KafkaConfiguration> options)
    {
        config = options.Value;

    }

    public async Task Produce(string topicName)
    {
        Console.WriteLine($"{nameof(Produce)} starting");
        CancellationTokenSource cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // The URL of the EventStreams service.
        string eventStreamsUrl = "https://stream.wikimedia.org/v2/stream/recentchange";
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        // Declare the producer reference here to enable calling the Flush
        // method in the finally block, when the app shuts down.
        IProducer<string, string>? producer = null;

        try
        {
            // Build a producer based on the provided configuration.
            // It will be disposed in the finally block.
            producer = new ProducerBuilder<string, string>(config).Build();
            int readerCount = 0;
            using var httpClient = new HttpClient();
            await using var stream = await httpClient.GetStreamAsync(eventStreamsUrl, cancellationToken);
            using var reader = new StreamReader(stream);
            // Read continuously until interrupted by Ctrl+C.
            while (!reader.EndOfStream /*&& readerCount < 1000*/)
            {
                var line = await reader.ReadLineAsync(cancellationToken);

                // The Wikimedia service sends a few lines, but the lines
                // of interest for this demo start with the "data:" prefix. 
                if (line is null || !line.StartsWith("data:"))
                {
                    continue;
                }

                // Extract and deserialize the JSON payload.
                int openBraceIndex = line.IndexOf('{');
                string jsonData = line.Substring(openBraceIndex);
                Console.WriteLine($"Data string: {jsonData}");

                // Parse the JSON to extract the URI of the edited page.
                var jsonDoc = JsonDocument.Parse(jsonData);
                var metaElement = jsonDoc.RootElement.GetProperty("meta");
                var uriElement = metaElement.GetProperty("uri");
                var key = uriElement.GetString()!; // Use the URI as the message key.

                // For higher throughput, use the non-blocking Produce call
                // and handle delivery reports out-of-band, instead of awaiting
                // the result of a ProduceAsync call.

                var message = new Message<string, string> { Key = key, Value = jsonData };
                await producer.ProduceAsync(topicName, message, cancellationToken);
                readerCount++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            var queueSize = producer?.Flush(TimeSpan.FromSeconds(5));
            if (queueSize > 0)
            {
                Console.WriteLine($"WARNING: Producer event queue has {queueSize} pending events on exit.");
            }
            producer?.Dispose();
        }
    }

}