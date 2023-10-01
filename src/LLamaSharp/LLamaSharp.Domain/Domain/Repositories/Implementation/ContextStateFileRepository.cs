using LLama;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Options;

using SerilogTimings.Extensions;

namespace LLamaSharp.Domain.Domain.Repositories.Implementation;

/// <summary>
/// Centralized handling for model state persist and load
/// Will probably evolve into something like repository to support both file systems and dbs
/// </summary>
public sealed class ContextStateFileRepository : IContextStateRepository
{
    private readonly ILogger logger;
    private readonly LlamaRepositoryOptions llamaRepositoryOptions;

    private const string SessionFolder = "session";
    private const string ModelFile = "model.bin";
    private const string ExecutorFile = "executor.bin";

    private static readonly string FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));


    /// <summary>
    /// Constructor for the Model State File Repository
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger">Serilog</param>
    public ContextStateFileRepository(IOptions<LlamaRepositoryOptions> options, ILogger logger)
    {
        this.logger = logger;
        llamaRepositoryOptions = options.Value;

    }
    private string GetPath(string userId)
    {
        var path = Path.GetFullPath(Path.Combine(FullPath, llamaRepositoryOptions.StatePersistencePath, userId));
        return path;
    }

    private string CreateAndGetPath(string userId)
    {
        var path = GetPath(userId);
        Directory.CreateDirectory(path!);
        return path;
    }

    public string GetFullPathName(string userId, string pathName)
    {
        var path = CreateAndGetPath(userId);
        return Path.Combine(path, pathName);
    }

    public void RemoveSessionState(string userId)
    {
        var path = CreateAndGetPath(userId);
        var folder = Path.Combine(path, SessionFolder);
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder);
        }
    }
    public void RemoveModelState(string userId)
    {
        var path = CreateAndGetPath(userId);
        var file = Path.Combine(path, ModelFile);
        File.Delete(file);
    }

    public void RemoveExecutorState(string userId)
    {
        var path = CreateAndGetPath(userId);
        var file = Path.Combine(path, ExecutorFile);
        File.Delete(file);
    }

    public void RemoveAllState(string userId)
    {
        RemoveExecutorState(userId);
        RemoveModelState(userId);
        RemoveSessionState(userId);
        var folder = GetPath(userId);
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder);
        }
    }

    public void SaveSession(ChatSession session, string userId, bool save)
    {
        if (save)
        {
            using var op = logger.BeginOperation("Saving Session State for {userId}...", userId);
            var folder = GetFullPathName(userId, SessionFolder);
            session.SaveSession(folder);
            op.Complete();
        }
    }

    public void LoadSession(ChatSession session, string userId, bool load)
    {
        if (load)
        {
            using var op = logger.BeginOperation("Loading Session State for {userId}...", userId);
            var folder = GetFullPathName(userId, SessionFolder);
            if (Directory.Exists(folder))
            {
                try
                {
                    session.LoadSession(folder);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error loading model Session for {userId}", userId);
                    Directory.Delete(folder!);
                }
            }
            op.Complete();
        }
    }

    /// <summary>
    /// Save state if filename is specified
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    public void SaveState(LLamaContext model, string userId, bool save)
    {
        if (save)
        {
            using var op = logger.BeginOperation("Saving Model State for {userId}...", userId);
            var fileName = GetFullPathName(userId, ModelFile);
            model.SaveState(fileName!);
            op.Complete();
        }
    }

    /// <summary>
    /// Save state for executor if filename is specified
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    public void SaveState(StatefulExecutorBase executor, string userId, bool save)
    {
        if (save)
        {
            using var op = logger.BeginOperation("Saving Model State for {userId}...", userId);
            var fileName = GetFullPathName(userId, ExecutorFile);
            executor.SaveState(fileName!);
            op.Complete();
        }
    }

    /// <summary>
    /// Load state if file exists
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    public void LoadState(LLamaContext model, string userId, bool load)
    {
        if (load)
        {
            using var op = logger.BeginOperation("Loading Model State for {userId}...", userId);
            var fileName = GetFullPathName(userId, ExecutorFile);
            if (File.Exists(fileName))
            {
                try
                {
                    model.LoadState(fileName!);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error loading model state for {userId}", userId);
                    File.Delete(fileName!);
                }
            }
            op.Complete();
        }
    }

    /// <summary>
    /// Load state for executor if file exists
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    public void LoadState(StatefulExecutorBase executor, string userId, bool load)
    {
        if (load)
        {
            using var op = logger.BeginOperation("Loading Model State for {userId}...", userId);
            var fileName = GetFullPathName(userId, ExecutorFile);
            if (File.Exists(fileName))
            {
                try
                {
                    executor.LoadState(fileName!);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error loading model state for {userId}", userId);
                    File.Delete(fileName!);
                }
            }
            op.Complete();
        }
    }
}
