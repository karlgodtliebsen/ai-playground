using AI.Domain.Configuration;
using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;
using Microsoft.Extensions.Options;

using SerilogTimings.Extensions;

using System.Net.Http.Json;

namespace AI.Domain.AIClients.Implementation;

public class FilesAIClient : AIClientBase, IFilesAIClient
{

    public FilesAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    private async Task<TR?> UploadFileAsync<TR>(string subUri, UploadFileRequest request, CancellationToken cancellationToken) where TR : class
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
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UploadAsync Failed {uri}", subUri);
        }

        return default;
    }

    public async Task<Response<Files>?> GetFilesAsync(CancellationToken cancellationToken)
    {
        var result = await GetAsync<Files>("files", cancellationToken);
        return new Response<Files>(result!);
    }

    public async Task<Response<FileData>?> UploadFilesAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadFileAsync<FileData>("files", request, cancellationToken);
        return new Response<FileData>(result!);
    }

    public async Task<Response<FileData>?> DeleteFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var result = await DeleteAsync<FileData>($"files/{fileId}", cancellationToken);
        return new Response<FileData>(result!);
    }

    public async Task<Response<FileData>?> RetrieveFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var result = await GetAsync<FileData>($"files/{fileId}", cancellationToken);
        return new Response<FileData>(result!);
    }

    public async Task<Response<string>?> RetrieveFileContentAsync(string fileId, CancellationToken cancellationToken)
    {
        var result = await GetContentAsync($"files/{fileId}/content", cancellationToken);
        return new Response<string>(result!);
    }
}