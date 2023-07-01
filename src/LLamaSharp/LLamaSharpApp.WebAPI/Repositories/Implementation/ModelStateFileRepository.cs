using LLama;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Repositories.Implementation;

/// <summary>
/// Centralized handling for model state persist and load
/// Will probably evolve into something like repository to support both file systems and dbs
/// </summary>
public class ModelStateFileRepository : IModelStateRepository
{
    private readonly WebApiOptions webApiOptions;

    private const string ModelFile = "model.bin";
    private const string ExecutorFile = "executor.bin";

    private readonly string fullPath;
    /// <summary>
    /// Constructor for the Model State File Repository
    /// </summary>
    /// <param name="options"></param>
    public ModelStateFileRepository(IOptions<WebApiOptions> options)
    {
        this.webApiOptions = options.Value;
        fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

    }
    private string GetFullPath(string userId)
    {
        var path = Path.GetFullPath(Path.Combine(fullPath, webApiOptions.StatePersistencePath, userId));
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
    public void SaveState(LLamaModel model, string userId, bool save)
    {
        if (save)
        {
            var fileName = GetFileName(userId, ModelFile);
            model.SaveState(fileName!);
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
            var fileName = GetFileName(userId, ExecutorFile);
            executor.SaveState(fileName!);
        }
    }

    /// <summary>
    /// Load state if file exists
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    public void LoadState(LLamaModel model, string userId, bool load)
    {
        if (load)
        {
            var fileName = GetFileName(userId, ExecutorFile);
            if (File.Exists(fileName))
            {
                model.LoadState(fileName!);
            }
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
            var fileName = GetFileName(userId, ExecutorFile);
            if (File.Exists(fileName))
            {
                executor.LoadState(fileName!);
            }
        }
    }
}
