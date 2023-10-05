using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;
using SemanticMemory.Kafka.StreamingNewsFeed.Domain;

namespace SemanticMemory.Kafka.StreamingNewsFeed.HostServices;

public class KafkaStreamingService : BackgroundService
{
    private readonly KafkaStreaming streaming;
    private readonly KafkaConfiguration settings;

    public KafkaStreamingService(KafkaStreaming streaming, IOptions<KafkaConfiguration> settings)
    {
        this.streaming = streaming;
        this.settings = settings.Value;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await streaming.UseStreams(settings.TopicSource, settings.TopicDestination);
    }
}
