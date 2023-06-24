using OpenAI.Client.Models.Files;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IFilesAIClient
{

    Task<Response<Files>?> GetFilesAsync(CancellationToken cancellationToken);
    Task<Response<FileData>?> UploadFilesAsync(UploadFileRequest request, CancellationToken cancellationToken);

    Task<Response<FileData>?> DeleteFileAsync(string fileId, CancellationToken cancellationToken);

    Task<Response<FileData>?> RetrieveFileAsync(string fileId, CancellationToken cancellationToken);

    Task<Response<string>?> RetrieveFileContentAsync(string fileId, CancellationToken cancellationToken);
}