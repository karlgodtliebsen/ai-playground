using System.Text.Json;
using Confluent.Kafka;
using Kernel.Memory.NewsFeed.Domain.Configuration;
using Kernel.Memory.NewsFeed.Domain.Domain;
using Microsoft.Extensions.Options;

namespace Kernel.Memory.NewsFeed.Producer.Host.Services;

public sealed class KafkaProducer : IKafkaProducer
{
    private readonly IStreamingClient client;
    private readonly ILogger logger;
    private readonly KafkaConfiguration configuration;
    const string EventStreamsUrl = "https://stream.wikimedia.org/v2/stream/recentchange";

    public KafkaProducer(IOptions<KafkaConfiguration> options, IStreamingClient client, ILogger logger)
    {
        this.client = client;
        this.logger = logger;
        configuration = options.Value;
    }

    public async Task Produce(CancellationToken cancellationToken)
    {
        string topicName = configuration.Topic;
        logger.Information("{name} starting", nameof(Produce));
        var retryCount = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunDownloadOfFeedAndProduceEvent(topicName, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Producing data from {url} for {topicName}", EventStreamsUrl, topicName);
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                retryCount++;
            }
            if (retryCount > 10)
            {
                logger.Error("Stopping Producing data from {url} after {retryCount} retries for {topicName}", EventStreamsUrl, retryCount, topicName);
                break;
            }
        }
    }

    private async Task RunDownloadOfFeedAndProduceEvent(string topicName, CancellationToken cancellationToken)
    {
        logger.Information("RunDownloadOfFeedAndProduceEvent {name} starting", nameof(Produce));

        // Build a producer based on the provided configuration.
        // It will be disposed in the finally block.
        using var producer = new ProducerBuilder<string, string>(configuration).Build();
        await using var stream = await client.GetStream(EventStreamsUrl, cancellationToken);
        using var reader = new StreamReader(stream);
        // Read continuously until interrupted by Ctrl+C.
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            // The Wikimedia service sends a few lines, but the lines
            // of interest for this demo start with the "data:" prefix. 
            if (line is null || !line.StartsWith("data:"))
            {
                continue;
            }
            var message = ProcessDataAndCreateMessage(line);
            await producer.ProduceAsync(topicName, message, cancellationToken);
        }

        var queueSize = producer.Flush(TimeSpan.FromSeconds(5));
        if (queueSize > 0)
        {
            logger.Information("WARNING: Producer event queue has {queueSize} pending events on exit.", queueSize);
        }
    }

    private Message<string, string> ProcessDataAndCreateMessage(string line)
    {
        // Extract and deserialize the JSON payload.
        var openBraceIndex = line.IndexOf('{');
        var jsonData = line.Substring(openBraceIndex);
        //logger.Information("Data string: {jsonData}", jsonData);

        // Parse the JSON to extract the URI of the edited page.
        var jsonDoc = JsonDocument.Parse(jsonData);
        var metaElement = jsonDoc.RootElement.GetProperty("meta");
        var uriElement = metaElement.GetProperty("uri");
        var key = uriElement.GetString()!; // Use the URI as the message key.

        // For higher throughput, use the non-blocking Produce call
        // and handle delivery reports out-of-band, instead of awaiting
        // the result of a ProduceAsync call.

        var message = new Message<string, string> { Key = key, Value = jsonData };
        return message;
    }

}
