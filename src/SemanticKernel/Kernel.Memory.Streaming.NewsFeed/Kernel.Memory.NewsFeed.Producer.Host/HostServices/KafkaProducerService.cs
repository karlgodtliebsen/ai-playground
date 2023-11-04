using Kernel.Memory.NewsFeed.Domain.Domain;
using Kernel.Memory.NewsFeed.Producer.Host.Services;
using Microsoft.Extensions.Hosting;

namespace Kernel.Memory.NewsFeed.Producer.Host.HostServices;

public class KafkaProducerService : BackgroundService
{
    private readonly IKafkaProducer producer;

    public KafkaProducerService(IKafkaProducer producer)
    {
        this.producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await producer.Produce(stoppingToken);
    }
}
