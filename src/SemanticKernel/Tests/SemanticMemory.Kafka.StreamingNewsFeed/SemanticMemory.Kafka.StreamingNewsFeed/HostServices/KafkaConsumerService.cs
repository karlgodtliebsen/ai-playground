using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WikiEditStream.Configuration;
using WikiEditStream.Domain;

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
        consumer.Consume(settings.TopicSource);
        return Task.CompletedTask;
    }
}