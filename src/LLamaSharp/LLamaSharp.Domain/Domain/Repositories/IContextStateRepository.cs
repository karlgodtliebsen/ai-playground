using LLama;

namespace LLamaSharp.Domain.Domain.Repositories;

/// <summary>
/// Centralized handling for model state persist and load
/// </summary>
public interface IContextStateRepository
{

    void RemoveAllState(string userId);
    void RemoveExecutorState(string userId);
    void RemoveModelState(string userId);

    void RemoveSessionState(string userId);

    /// <summary>
    /// Gets the full path for the specified user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="pathName"></param>
    /// <returns></returns>
    string GetFullPathName(string userId, string pathName);

    /// <summary>
    /// Saves the state of the model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    void SaveState(LLamaContext model, string userId, bool save);

    /// <summary>
    /// Saves the state of the executor
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    void SaveState(StatefulExecutorBase executor, string userId, bool save);

    /// <summary>
    /// Saves the session state for the specified  user
    /// </summary>
    /// <param name="session"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    void SaveSession(ChatSession session, string userId, bool save);

    /// <summary>
    /// Loads the state of the model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    void LoadState(LLamaContext model, string userId, bool load);

    /// <summary>
    /// Loads the state of the executor
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    void LoadState(StatefulExecutorBase executor, string userId, bool load);

    /// <summary>
    /// Loads the session state for the specified  user
    /// </summary>
    /// <param name="session"></param>
    /// <param name="userId"></param>
    /// <param name="load"></param>
    void LoadSession(ChatSession session, string userId, bool load);
}
