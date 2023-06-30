using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Repositories;

/// <summary>
/// 
/// </summary>
public interface IUsersStateRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    Task PersistInferenceOptions(InferenceOptions options, string userId, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    Task PersistLlmaModelOptions(LlmaModelOptions options, string userId, CancellationToken cancellationToken);
    /// <summary>
    /// G
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<InferenceOptions?> GetInferenceOptions(string userId, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<LlmaModelOptions?> GetLlmaModelOptions(string userId, CancellationToken cancellationToken);
}
