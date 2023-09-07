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

    private string GetFullPath(string userId)
    {
        var path = Path.GetFullPath(Path.Combine(FullPath, llamaRepositoryOptions.StatePersistencePath, userId));
        Directory.CreateDirectory(path!);
        return path;
    }

    private string GetFileName(string userId, string fileName)
    {
        var path = GetFullPath(userId);
        return Path.Combine(path, fileName);
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
            var fileName = GetFileName(userId, ModelFile);
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
            var fileName = GetFileName(userId, ExecutorFile);
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
            var fileName = GetFileName(userId, ExecutorFile);
            if (File.Exists(fileName))
            {
                model.LoadState(fileName!);
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
            var fileName = GetFileName(userId, ExecutorFile);
            if (File.Exists(fileName))
            {
                executor.LoadState(fileName!);
            }
            op.Complete();
        }
    }
}
