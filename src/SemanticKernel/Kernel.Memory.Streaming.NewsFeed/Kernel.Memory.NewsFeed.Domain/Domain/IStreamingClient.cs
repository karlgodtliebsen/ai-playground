namespace Kernel.Memory.NewsFeed.Domain.Domain;

public interface IStreamingClient
{
    Task<Stream> GetStream(string url, CancellationToken cancellationToken);
}
