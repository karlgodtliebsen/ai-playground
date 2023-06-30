using LLama;

namespace LLamaSharpApp.WebAPI.Repositories;

/// <summary>
/// Centralized handling for model state persist and load
/// </summary>
public interface IModelStateRepository
{
    void SaveState(LLamaModel model, Func<string?> save);
    void SaveState(StatefulExecutorBase executor, Func<string?> save);
    void LoadState(LLamaModel model, Func<string?> load);
    void LoadState(StatefulExecutorBase executor, Func<string?> load);
}
