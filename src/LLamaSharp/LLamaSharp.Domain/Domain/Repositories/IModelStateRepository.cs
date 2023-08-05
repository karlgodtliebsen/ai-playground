using LLama;

namespace LLamaSharp.Domain.Domain.Repositories;

/// <summary>
/// Centralized handling for model state persist and load
/// </summary>
public interface IModelStateRepository
{
    /// <summary>
    /// Saves the state of the model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    void SaveState(LLamaModel model, string userId, bool save);

    /// <summary>
    /// Saves the state of the executor
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    void SaveState(StatefulExecutorBase executor, string userId, bool save);

    /// <summary>
    /// Loads the state of the model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    void LoadState(LLamaModel model, string userId, bool load);

    /// <summary>
    /// Loads the state of the executor
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    void LoadState(StatefulExecutorBase executor, string userId, bool load);
}
