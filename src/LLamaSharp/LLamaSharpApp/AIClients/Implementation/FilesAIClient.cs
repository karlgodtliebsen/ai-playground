using System.Net.Http.Json;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models.Files;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

using SerilogTimings.Extensions;

namespace OpenAI.Client.AIClients.Implementation;

public class FilesAIClient : AIClientBase, IFilesAIClient
{

    public FilesAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    private async Task<OneOf<TR, ErrorResponse>> UploadFileAsync<TR>(string subUri, UploadFileRequest request, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("UploadFileAsync", subUri);
        try
        {
            PrepareClient();
            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Purpose), "purpose" },
                { new ByteArrayContent(await File.ReadAllBytesAsync(request.FullFilename, cancellationToken)), "file", Path.GetFileName(request.FullFilename) }
            };
            var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TR>(cancellationToken: cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UploadAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    public async Task<OneOf<Files, ErrorResponse>> GetFilesAsync(CancellationToken cancellationToken)
    {
        var result = await GetAsync<Files>("files", cancellationToken);
        return result;
    }

    public async Task<OneOf<FileData, ErrorResponse>> UploadFilesAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadFileAsync<FileData>("files", request, cancellationToken);
        return result;
    }

    public async Task<OneOf<FileData, ErrorResponse>> DeleteFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var result = await DeleteAsync<FileData>($"files/{fileId}", cancellationToken);
        return result;
    }

    public async Task<OneOf<FileData, ErrorResponse>> RetrieveFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<FileData>($"files/{fileId}", cancellationToken);
        return result;
    }

    public async Task<OneOf<string, ErrorResponse>> RetrieveFileContentAsync(string fileId, CancellationToken cancellationToken)
    {
        var result = await GetContentAsync($"files/{fileId}/content", cancellationToken);
        return result;
    }
}
