using Microsoft.Extensions.Logging;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.ContentStorage;
using Microsoft.SemanticMemory.ContentStorage.DevTools;
using Microsoft.SemanticMemory.Diagnostics;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

public class CustomContentStorage : IContentStorage
{
    // Parent directory of the directory containing messages
    private readonly string _directory;

    // Application logger
    private readonly ILogger<CustomContentStorage> _log;

    public CustomContentStorage(SimpleFileStorageConfig config, ILogger<CustomContentStorage>? log = null)
    {
        this._log = log ?? DefaultLogger<CustomContentStorage>.Instance;
        this.CreateDirectory(config.Directory);
        this._directory = config.Directory;
    }

    /// <inherit />
    public Task CreateIndexDirectoryAsync(string index, CancellationToken cancellationToken = default)
    {
        this.CreateDirectory(Path.GetFullPath(Path.Join(this._directory, index)));
        return Task.CompletedTask;
    }

    /// <inherit />
    public async Task CreateDocumentDirectoryAsync(
        string index,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        await this.CreateIndexDirectoryAsync(index, cancellationToken).ConfigureAwait(false);
        this.CreateDirectory(Path.Join(this._directory, index, documentId));
    }

    /// <inherit />
    public async Task WriteTextFileAsync(
        string index,
        string documentId,
        string fileName,
        string fileContent,
        CancellationToken cancellationToken = default)
    {
        await this.CreateDocumentDirectoryAsync(index, documentId, cancellationToken).ConfigureAwait(false);
        fileName = Path.GetFileName(fileName);
        var fullFileName = Path.GetFullPath(Path.Join(this._directory, index, documentId, fileName));
        this._log.LogDebug("Writing file {0}", fullFileName);
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
        await this.CreateDocumentDirectoryAsync(index, documentId, cancellationToken).ConfigureAwait(false);
        var fullFileName = Path.GetFullPath(Path.Join(this._directory, index, documentId, fileName));
        this._log.LogDebug("Creating file {0}", fullFileName);
        // ReSharper disable once UseAwaitUsing
        using var outputStream = File.Create(fullFileName);
        contentStream.Seek(0, SeekOrigin.Begin);
        this._log.LogDebug("Writing to file {0}", fullFileName);
        await contentStream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
        return outputStream.Length;
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
        var fullFileName = Path.GetFullPath(Path.Join(this._directory, index, documentId, fileName));
        if (!File.Exists(fullFileName))
        {
            if (errIfNotFound) { this._log.LogError("File not found {0}", fullFileName); }

            throw new ContentStorageFileNotFoundException("File not found");
        }

        byte[] data = File.ReadAllBytes(fullFileName);
        return Task.FromResult(new BinaryData(data));
    }

    /// <inherit />
    public Task DeleteDocumentDirectoryAsync(
        string index,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Join(this._directory, index, documentId);
        path = Path.GetFullPath(path);
        string[] files = Directory.GetFiles(path);
        foreach (string fileName in files)
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
            this._log.LogDebug("Creating directory {0}", path);
            Directory.CreateDirectory(path);
        }
    }
}
