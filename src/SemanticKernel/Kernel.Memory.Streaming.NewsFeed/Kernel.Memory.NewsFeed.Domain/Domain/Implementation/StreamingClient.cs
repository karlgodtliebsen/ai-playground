namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public sealed class StreamingClient : IStreamingClient
{
    private readonly HttpClient httpClient;

    public StreamingClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<Stream> GetStream(string url, CancellationToken cancellationToken)
    {
        return await httpClient.GetStreamAsync(url, cancellationToken);
    }

}