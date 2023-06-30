using LLama;

namespace LLamaSharpApp.WebAPI.Services;

/// <summary>
/// Centralized handling for model state persist and load
/// </summary>
public class StateHandler : IStateHandler
{

    //is state exists then we load it
    //    model.LoadState(fileName);//we must identify the user and look up in db or file system for the saved state.
    //Should we do some mapping from session7userid to storage name
    //we cannot relay on easily hacked session id - username etc.

    public void SaveState(LLamaModel model, Func<string?> save)
    {
        var fileName = save();
        if (fileName is not null)
        {
            model.SaveState(fileName!);
        }
    }

    public void SaveState(StatefulExecutorBase executor, Func<string?> save)
    {
        var fileName = save();
        if (fileName is not null)
        {
            executor.SaveState(fileName);
        }
    }

    public void LoadState(LLamaModel model, Func<string?> load)
    {
        var fileName = load();
        if (fileName is not null && File.Exists(fileName))
        {
            model.LoadState(fileName!);
        }
    }

    public void LoadState(StatefulExecutorBase executor, Func<string?> load)
    {
        var fileName = load();
        if (fileName is not null && File.Exists(fileName))
        {
            executor.LoadState(fileName);
        }
    }


}
