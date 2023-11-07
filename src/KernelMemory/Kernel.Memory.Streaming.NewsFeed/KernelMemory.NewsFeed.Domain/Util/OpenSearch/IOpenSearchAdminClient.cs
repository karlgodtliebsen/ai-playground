namespace Kernel.Memory.NewsFeed.Domain.Util.OpenSearch;

public interface IOpenSearchAdminClient
{
    Task CreateIndicesAsync(CancellationToken cancellationToken);
    Task CreateIndexAsync(string index, CancellationToken cancellationToken);
}
