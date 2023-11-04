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

    public async Task CreateIndexAsync(string index, CancellationToken cancellationToken)
    {
        PostData body = "";
        var responseGraph = await openSearchLowLevelClient.Indices.CreateAsync<StringResponse>(index, body, ctx: cancellationToken);
        CheckForErrors(responseGraph.DebugInformation);
    }

    private void CheckForErrors(string response)
    {
        var ok = response.Contains("(200)") || response.Contains("(201)") || response.Contains("(2");
        if (!ok)
        {
            logger.Debug(response);
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
        CheckForErrors(responseGraph.DebugInformation);
        return fileContent.Length;
    }

    public async Task WriteFileAsync(string index, string documentId, string fileName, Stream contentStream, CancellationToken cancellationToken = new CancellationToken())
    {
        contentStream.Position = 0;
        using var reader = new StreamReader(contentStream, leaveOpen: false);
        var fileContent = await reader.ReadToEndAsync(cancellationToken);
        var responseGraph = await openSearchLowLevelClient.IndexAsync<StringResponse>(index, documentId, fileContent, ctx: cancellationToken);
        CheckForErrors(responseGraph.DebugInformation);
    }


    const string SearchTemplate = """{"query": {"match": {"_id": "#id#"}}}""";
    const string PlaceHolder = "#id#";
    private string CreateSearchExpression(string id)
    {
        string pd = SearchTemplate.Replace(PlaceHolder, id);
        return pd;
    }

    protected async Task<T?> Find<T>(string index, string id, CancellationToken cancellationToken) where T : class
    {
        //https://opensearch.org/docs/2.11/clients/OSC-example
        var parameters = new SearchRequestParameters();
        string pd = CreateSearchExpression(id);
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

        CreateIndex(index);

        var data = await Find<string>(index, documentId, cancellationToken);

        //TODO: check if data is null
        return new BinaryData(Array.Empty<byte>());
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
