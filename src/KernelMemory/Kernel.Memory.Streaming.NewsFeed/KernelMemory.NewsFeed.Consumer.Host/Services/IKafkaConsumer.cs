namespace Kernel.Memory.NewsFeed.Domain.Domain;

public interface IKafkaConsumer
{
    Task Consume(CancellationToken cancellationToken);
}
