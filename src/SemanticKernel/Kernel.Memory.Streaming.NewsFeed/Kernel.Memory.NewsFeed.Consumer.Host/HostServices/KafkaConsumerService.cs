using Kernel.Memory.NewsFeed.Domain.Domain;
using Microsoft.Extensions.Hosting;

namespace Kernel.Memory.NewsFeed.Host.HostServices;

public class KafkaConsumerService : BackgroundService
{
    private readonly IKafkaConsumer consumer;

    public KafkaConsumerService(IKafkaConsumer consumer)
    {
        this.consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.Consume(stoppingToken);
    }
}
