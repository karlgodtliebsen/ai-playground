namespace Kernel.Memory.NewsFeed.Producer.Host.Services;

public interface IKafkaProducer
{
    Task Produce(CancellationToken cancellationToken);
}
