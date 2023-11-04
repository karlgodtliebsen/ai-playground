using AI.Library.Utils;

using Kernel.Memory.NewsFeed.Domain.Configuration;

using Microsoft.Extensions.Options;

using OpenSearch.Net;

namespace Kernel.Memory.NewsFeed.Domain.Util.OpenSearch;

public class OpenSearchAdminClient : IOpenSearchAdminClient
{
    private readonly IOpenSearchLowLevelClient openSearchLowLevelClient;
    private readonly OpenSearchConfiguration openSearchConfiguration;

    // Application logger
    private readonly ILogger logger;

    public OpenSearchAdminClient(
        IOpenSearchLowLevelClient openSearchLowLevelClient,
        IOptions<OpenSearchConfiguration> openSearchOptions,
        ILogger logger)
    {
        this.openSearchLowLevelClient = openSearchLowLevelClient;
        openSearchConfiguration = openSearchOptions.Value;
        this.logger = logger;
    }

    public async Task CreateIndicesAsync(CancellationToken cancellationToken)
    {
        foreach (var index in openSearchConfiguration.Indices)
        {
            await CreateIndexAsync(index, cancellationToken);
        }
    }

    public async Task CreateIndexAsync(string index, CancellationToken cancellationToken)
    {
        //The index exists API operation returns only one of two possible response codes: 200 – the index exists, and 404 – the index does not exist.
        var exist = await openSearchLowLevelClient.Indices.ExistsAsync<StringResponse>(index, ctx: cancellationToken);
        if (CheckResponse(exist.DebugInformation)) return;


        PostData pd = new IndexConfiguration().ToJson();
        var responseGraph = await openSearchLowLevelClient.Indices.CreateAsync<StringResponse>(index, pd, ctx: cancellationToken);
        CheckForErrors(responseGraph);
    }

    private bool CheckResponse(string response)
    {
        return response.Contains("(200)");
    }

    private void CheckForErrors(StringResponse? response)
    {
        if (response is null)
        {
            throw new OpenSearchException("Response is null");
            logger.Debug(response.DebugInformation);
        }

        if (!response.Success)
        {
            throw new OpenSearchException("Request Failed ");
            logger.Debug(response.DebugInformation);
        }

        CheckForErrors(response.DebugInformation);
    }

    private void CheckForErrors(string response)
    {
        var ok = response.Contains("(200)") || response.Contains("(201)") || response.Contains("(2");
        if (!ok)
        {
            logger.Debug(response);
            throw new OpenSearchException("Request Failed ");
        }
    }

}
