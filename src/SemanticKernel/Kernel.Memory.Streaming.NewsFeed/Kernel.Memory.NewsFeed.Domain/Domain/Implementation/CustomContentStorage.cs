using Microsoft.Extensions.Logging;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.ContentStorage;
using Microsoft.SemanticMemory.ContentStorage.DevTools;
using Microsoft.SemanticMemory.Diagnostics;

namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public sealed class CustomContentStorage : IContentStorage
{
    // Parent directory of the directory containing messages
    private readonly string _directory;

    // Application logger
    private readonly ILogger<CustomContentStorage> _log;

    public CustomContentStorage(SimpleFileStorageConfig config, ILogger<CustomContentStorage>? log = null)
    {
        _log = log ?? DefaultLogger<CustomContentStorage>.Instance;
        CreateDirectory(config.Directory);
        _directory = config.Directory;
    }

    /// <inherit />
    public Task CreateIndexDirectoryAsync(string index, CancellationToken cancellationToken = default)
    {
        CreateDirectory(Path.GetFullPath(Path.Join(_directory, index)));
        return Task.CompletedTask;
    }

    public Task DeleteIndexDirectoryAsync(string index, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    /// <inherit />
    public async Task CreateDocumentDirectoryAsync(
        string index,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        await CreateIndexDirectoryAsync(index, cancellationToken).ConfigureAwait(false);
        CreateDirectory(Path.Join(_directory, index, documentId));
    }

    public Task EmptyDocumentDirectoryAsync(string index, string documentId, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    /// <inherit />
    public async Task WriteTextFileAsync(
        string index,
        string documentId,
        string fileName,
        string fileContent,
        CancellationToken cancellationToken = default)
    {
        await CreateDocumentDirectoryAsync(index, documentId, cancellationToken).ConfigureAwait(false);
        fileName = Path.GetFileName(fileName);
        var fullFileName = Path.GetFullPath(Path.Join(_directory, index, documentId, fileName));
        _log.LogDebug("Writing file {0}", fullFileName);
        await File.WriteAllTextAsync(fullFileName, fileContent, cancellationToken).ConfigureAwait(false);
    }

    /// <inherit />
    public async Task<long> WriteStreamAsync(
        string index,
        string documentId,
        string fileName,
        Stream contentStream,
        CancellationToken cancellationToken = default)
    {
        fileName = Path.GetFileName(fileName);
        await CreateDocumentDirectoryAsync(index, documentId, cancellationToken).ConfigureAwait(false);
        var fullFileName = Path.GetFullPath(Path.Join(_directory, index, documentId, fileName));
        _log.LogDebug("Creating file {0}", fullFileName);
        // ReSharper disable once UseAwaitUsing
        using var outputStream = File.Create(fullFileName);
        contentStream.Seek(0, SeekOrigin.Begin);
        _log.LogDebug("Writing to file {0}", fullFileName);
        await contentStream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
        return outputStream.Length;
    }

    public Task WriteFileAsync(string index, string documentId, string fileName, Stream streamContent, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    /// <inherit />
    public Task<BinaryData> ReadFileAsync(
        string index,
        string documentId,
        string fileName,
        bool errIfNotFound = true,
        CancellationToken cancellationToken = default)
    {
        fileName = Path.GetFileName(fileName);
        var fullFileName = Path.GetFullPath(Path.Join(_directory, index, documentId, fileName));
        if (!File.Exists(fullFileName))
        {
            if (errIfNotFound) { _log.LogError("File not found {0}", fullFileName); }

            throw new ContentStorageFileNotFoundException("File not found");
        }

        var data = File.ReadAllBytes(fullFileName);
        return Task.FromResult(new BinaryData(data));
    }

    /// <inherit />
    public Task DeleteDocumentDirectoryAsync(
        string index,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Join(_directory, index, documentId);
        path = Path.GetFullPath(path);
        var files = Directory.GetFiles(path);
        foreach (var fileName in files)
        {
            // Don't delete the pipeline status file
            if (fileName == Constants.PipelineStatusFilename) { continue; }

            File.Delete(fileName);
        }

        return Task.CompletedTask;
    }

    private void CreateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = Path.GetFullPath(path);
        if (!Directory.Exists(path))
        {
            _log.LogDebug("Creating directory {0}", path);
            Directory.CreateDirectory(path);
        }
    }
}

