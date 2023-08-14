using LLamaSharp.Domain.Configuration;

namespace LLamaSharp.Domain.Domain.Services;

/// <summary>
/// Inference for the Options Service
/// </summary>
public interface IOptionsService
{
    /// <summary>
    /// Returns the available models
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetModels();

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
    Task PersistLlamaModelOptions(LlamaModelOptions? options, string userId, CancellationToken cancellationToken);

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
    Task<LlamaModelOptions> GetLlamaModelOptions(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get a snapshot of the default Llama model options
    /// </summary>
    /// <returns></returns>
    LlamaModelOptions GetDefaultLlamaModelOptions();

    /// <summary>
    /// Get a snapshot of the default Inference options
    /// </summary>
    /// <returns></returns>
    InferenceOptions GetDefaultInferenceOptions();


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
    Task<LlamaModelOptions> CoalsceLlamaModelOptions(LlamaModelOptions? queryOptions, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get the system prompt templates
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> GetSystemPromptTemplates(CancellationToken cancellationToken);

    /// <summary>
    /// get the specified system prompt templates
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetSpecifiedSystemPromptTemplates(string name, string? version = null, CancellationToken? cancellationToken = null);
}

