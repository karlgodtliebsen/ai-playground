using OneOf;
using OpenAI.Client.OpenAI.Models.Files;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IFilesAIClient
{

    Task<OneOf<Files, ErrorResponse>> GetFilesAsync(CancellationToken cancellationToken);
    Task<OneOf<FileData, ErrorResponse>> UploadFilesAsync(UploadFileRequest request, CancellationToken cancellationToken);

    Task<OneOf<FileData, ErrorResponse>> DeleteFileAsync(string fileId, CancellationToken cancellationToken);

    Task<OneOf<FileData, ErrorResponse>> RetrieveFileAsync(string fileId, CancellationToken cancellationToken);

    Task<OneOf<string, ErrorResponse>> RetrieveFileContentAsync(string fileId, CancellationToken cancellationToken);
}

