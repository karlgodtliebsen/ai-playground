using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Semantic.Memory.Kafka.Streaming.NewsFeed.Configuration;
using Semantic.Memory.Kafka.Streaming.NewsFeed.Domain;

namespace Semantic.Memory.Kafka.Streaming.NewsFeed.HostServices;

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
