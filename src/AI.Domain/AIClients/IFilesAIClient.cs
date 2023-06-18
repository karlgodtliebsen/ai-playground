using AI.Domain.Models;
using AI.Domain.Models.Requests;
using AI.Domain.Models.Responses;

namespace AI.Domain.AIClients;

public interface IFilesAIClient
{

    Task<Response<Files>?> GetFilesAsync(CancellationToken cancellationToken);
    Task<Response<FileData>?> UploadFilesAsync(UploadFileRequest request, CancellationToken cancellationToken);

    Task<Response<FileData>?> DeleteFileAsync(string fileId, CancellationToken cancellationToken);

    Task<Response<FileData>?> RetrieveFileAsync(string fileId, CancellationToken cancellationToken);

    Task<Response<string>?> RetrieveFileContentAsync(string fileId, CancellationToken cancellationToken);


}