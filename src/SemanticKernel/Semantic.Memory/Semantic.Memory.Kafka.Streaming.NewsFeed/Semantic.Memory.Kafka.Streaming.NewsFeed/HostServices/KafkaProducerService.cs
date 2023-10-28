using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Semantic.Memory.Kafka.Streaming.NewsFeed.Configuration;
using Semantic.Memory.Kafka.Streaming.NewsFeed.Domain;

namespace Semantic.Memory.Kafka.Streaming.NewsFeed.HostServices;

public class KafkaProducerService : BackgroundService
{
    private readonly KafkaProducer producer;
    private readonly KafkaConfiguration settings;

    public KafkaProducerService(KafkaProducer producer, IOptions<KafkaConfiguration> settings)
    {
        this.producer = producer;
        this.settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await producer.Produce(settings.TopicProducer, stoppingToken);
    }
}
