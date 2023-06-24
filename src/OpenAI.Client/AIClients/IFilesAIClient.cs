using OneOf;

using OpenAI.Client.Models.Files;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IFilesAIClient
{

    Task<OneOf<Files, ErrorResponse>> GetFilesAsync(CancellationToken cancellationToken);
    Task<OneOf<FileData, ErrorResponse>> UploadFilesAsync(UploadFileRequest request, CancellationToken cancellationToken);

    Task<OneOf<FileData, ErrorResponse>> DeleteFileAsync(string fileId, CancellationToken cancellationToken);

    Task<OneOf<FileData, ErrorResponse>> RetrieveFileAsync(string fileId, CancellationToken cancellationToken);

    Task<OneOf<string, ErrorResponse>> RetrieveFileContentAsync(string fileId, CancellationToken cancellationToken);
}

