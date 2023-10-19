using System.Text.Json;

using Confluent.Kafka;

using Microsoft.Extensions.Options;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

public class KafkaProducer
{
    private readonly ILogger logger;
    private readonly KafkaConfiguration config;
    const string EventStreamsUrl = "https://stream.wikimedia.org/v2/stream/recentchange";

    public KafkaProducer(IOptions<KafkaConfiguration> options, ILogger logger)
    {
        this.logger = logger;
        config = options.Value;
    }

    public async Task Produce(string topicName, CancellationToken cancellationToken)
    {
        logger.Information("{name} starting", nameof(Produce));
        int retryCount = 0;
        while (retryCount <= 10)
        {
            try
            {
                await ResilientProducer(topicName, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Producing data from {url}", EventStreamsUrl);
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                retryCount++;
            }
            if (retryCount > 10)
            {
                logger.Error("Stopping Producing data from {url} after {retryCount} retries", EventStreamsUrl, retryCount);
                break;
            }
        }
    }

    private async Task ResilientProducer(string topicName, CancellationToken cancellationToken)
    {
        logger.Information("{name} starting", nameof(Produce));

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
            await using var stream = await httpClient.GetStreamAsync(EventStreamsUrl, cancellationToken);
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
                logger.Information("Data string: {jsonData}", jsonData);

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
            logger.Error(ex, "Error Producing data from {url}", EventStreamsUrl);
            throw;
        }
        finally
        {
            var queueSize = producer?.Flush(TimeSpan.FromSeconds(5));
            if (queueSize > 0)
            {
                logger.Information("WARNING: Producer event queue has {queueSize} pending events on exit.", queueSize);
            }
            producer?.Dispose();
        }
    }

}
