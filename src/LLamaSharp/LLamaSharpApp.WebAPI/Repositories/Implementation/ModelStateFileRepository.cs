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
    private readonly WebApiOptions options;

    /// <summary>
    /// Constructor for the Model State File Repository
    /// </summary>
    /// <param name="options"></param>
    public ModelStateFileRepository(IOptions<WebApiOptions> options)
    {
        this.options = options.Value;
    }

    //is state exists then we load it
    //    model.LoadState(fileName);//we must identify the user and look up in db or file system for the saved state.
    //Should we do some mapping from session7userid to storage name
    //we cannot relay on easily hacked session id - username etc.

    /// <summary>
    /// Save state if filename is specified
    /// </summary>
    /// <param name="model"></param>
    /// <param name="save"></param>
    public void SaveState(LLamaModel model, Func<string?> save)
    {
        var fileName = save();
        if (fileName is not null)
        {
            model.SaveState(fileName!);
        }
    }

    /// <summary>
    /// Save state for executor if filename is specified
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="save"></param>
    public void SaveState(StatefulExecutorBase executor, Func<string?> save)
    {
        var fileName = save();
        if (fileName is not null)
        {
            executor.SaveState(fileName);
        }
    }

    /// <summary>
    /// Load state if file exists
    /// </summary>
    /// <param name="model"></param>
    /// <param name="load"></param>
    public void LoadState(LLamaModel model, Func<string?> load)
    {
        var fileName = load();
        if (fileName is not null && File.Exists(fileName))
        {
            model.LoadState(fileName!);
        }
    }

    /// <summary>
    /// Load state for executor if file exists
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="load"></param>
    public void LoadState(StatefulExecutorBase executor, Func<string?> load)
    {
        var fileName = load();
        if (fileName is not null && File.Exists(fileName))
        {
            executor.LoadState(fileName);
        }
    }
}
