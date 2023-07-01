using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Services;

/// <summary>
/// Inference for the Options Service
/// </summary>
public interface IOptionsService
{
    /// <summary>
    /// Persist Inference Options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    Task PersistInferenceOptions(InferenceOptions? options, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Persist Llma Model Options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    Task PersistLlmaModelOptions(LlmaModelOptions? options, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Inference Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InferenceOptions> GetInferenceOptions(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Llma Model Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LlmaModelOptions> GetLlmaModelOptions(string userId, CancellationToken cancellationToken);


    /// <summary>
    /// This method unites the options that might be submitted for this specific call, or that might be stored for this specific user,
    /// or the default options if none of the former exist, to form one whole or to merge into a single entity.
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InferenceOptions> CoalsceInferenceOptions(InferenceOptions? queryOptions, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// This method unites the options that might be submitted for this specific call, or that might be stored for this specific user,
    /// or the default options if none of the former exist, to form one whole or to merge into a single entity.
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LlmaModelOptions> CoalsceLlmaModelOptions(LlmaModelOptions? queryOptions, string userId, CancellationToken cancellationToken);
}
