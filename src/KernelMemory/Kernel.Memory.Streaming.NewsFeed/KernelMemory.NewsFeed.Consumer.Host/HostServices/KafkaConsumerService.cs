using Kernel.Memory.NewsFeed.Domain.Domain;
using Kernel.Memory.NewsFeed.Domain.Util.OpenSearch;
using Microsoft.Extensions.Hosting;

namespace Kernel.Memory.NewsFeed.Host.HostServices;

public class KafkaConsumerService : BackgroundService
{
    private readonly IKafkaConsumer consumer;
    private readonly IOpenSearchAdminClient adminClient;

    public KafkaConsumerService(IKafkaConsumer consumer, IOpenSearchAdminClient adminClient)
    {
        this.consumer = consumer;
        this.adminClient = adminClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await adminClient.CreateIndicesAsync(cancellationToken);
        await consumer.Consume(cancellationToken);
    }
}
