namespace Kernel.Memory.NewsFeed.Domain.Domain;

public interface IEventsToSemanticMemory
{
    Task AddMessage(string key, string message, CancellationToken cancellationToken);
}
