using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;
using SemanticMemory.Kafka.StreamingNewsFeed.Domain;

namespace SemanticMemory.Kafka.StreamingNewsFeed.HostServices;

public class KafkaConsumerService : BackgroundService
{
    private readonly KafkaConsumer consumer;
    private readonly KafkaConfiguration settings;

    public KafkaConsumerService(KafkaConsumer consumer, IOptions<KafkaConfiguration> settings)
    {
        this.consumer = consumer;
        this.settings = settings.Value;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Consume(settings.TopicProducer, stoppingToken);
        return Task.CompletedTask;
    }
}
