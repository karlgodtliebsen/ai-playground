using Kernel.Memory.NewsFeed.Domain.Util;

using Microsoft.SemanticMemory.ContentStorage;

using OpenSearch.Client;
using OpenSearch.Net;

namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public class OpenSearchCustomContentStorage : IContentStorage
{
    private readonly IOpenSearchClient openSearchClient;
    private readonly IOpenSearchLowLevelClient openSearchLowLevelClient;

    // Application logger
    private readonly ILogger logger;

    public OpenSearchCustomContentStorage(
        IOpenSearchClient openSearchClient,
        IOpenSearchLowLevelClient openSearchLowLevelClient,
        ILogger logger)
    {
        this.openSearchClient = openSearchClient;
        this.openSearchLowLevelClient = openSearchLowLevelClient;
        this.logger = logger;

    }

    /// <inherit />
    public Task CreateIndexDirectoryAsync(string index, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteIndexDirectoryAsync(string index, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    /// <inherit />
    public Task CreateDocumentDirectoryAsync(
        string index,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task EmptyDocumentDirectoryAsync(string index, string documentId, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
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
            throw new OpenSearchException("Request Failed \n" + response);
        }
    }

    /// <inherit />
    public async Task WriteTextFileAsync(
        string index,
        string documentId,
        string fileName,
        string fileContent,
        CancellationToken cancellationToken = default)
    {
        var responseGraph = await openSearchLowLevelClient.IndexAsync<StringResponse>(index, documentId, fileContent, ctx: cancellationToken);
        CheckForErrors(responseGraph.DebugInformation);
    }

    /// <inherit />
    public async Task<long> WriteStreamAsync(
        string index,
        string documentId,
        string fileName,
        Stream contentStream,
        CancellationToken cancellationToken = default)
    {
        contentStream.Position = 0;
        using var reader = new StreamReader(contentStream, leaveOpen: false);
        var fileContent = await reader.ReadToEndAsync(cancellationToken);
        var responseGraph = await openSearchLowLevelClient.IndexAsync<StringResponse>(index, documentId, fileContent, ctx: cancellationToken);
        CheckForErrors(responseGraph);
        return fileContent.Length;
    }

    public async Task WriteFileAsync(string index, string documentId, string fileName, Stream contentStream, CancellationToken cancellationToken = new CancellationToken())
    {
        contentStream.Position = 0;
        using var reader = new StreamReader(contentStream, leaveOpen: false);
        var fileContent = await reader.ReadToEndAsync(cancellationToken);
        var responseGraph = await openSearchLowLevelClient.IndexAsync<StringResponse>(index, documentId, fileContent, ctx: cancellationToken);
        CheckForErrors(responseGraph);
    }

    const string SearchTemplate = """{"query": {"match": {"_id": "#id#"}}}""";
    const string PlaceHolder = "#id#";

    private string CreateSearchExpression(string id)
    {
        var pd = SearchTemplate.Replace(PlaceHolder, id);
        return pd;
    }

    protected async Task<T?> Find<T>(string index, string id, CancellationToken cancellationToken) where T : class
    {
        //https://opensearch.org/docs/2.11/clients/OSC-example
        var parameters = new SearchRequestParameters();
        var pd = CreateSearchExpression(id);
        var searchResponse = await openSearchLowLevelClient.SearchAsync<SearchResponse<T>>(index, pd, parameters, cancellationToken);

        CheckForErrors(searchResponse.ApiCall.DebugInformation);
        if (searchResponse.Documents.Count == 0) return default;
        return searchResponse.Documents.FirstOrDefault()!;
    }

    /// <inherit />
    public async Task<BinaryData> ReadFileAsync(
        string index,
        string documentId,
        string fileName,
        bool errIfNotFound = true,
        CancellationToken cancellationToken = default)
    {
        var data = await Find<object>(index, documentId, cancellationToken);
        if (data is null) return default;


        return new BinaryData(data);
    }

    /// <inherit />
    public Task DeleteDocumentDirectoryAsync(
        string index,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }


}
