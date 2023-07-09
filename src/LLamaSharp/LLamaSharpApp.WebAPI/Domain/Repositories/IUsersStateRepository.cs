﻿using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Domain.Repositories;

/// <summary>
/// Handles Users State for InferenceOptions and LlamaModelOptions
/// </summary>
public interface IUsersStateRepository
{
    /// <summary>
    /// Persist Inference Options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    Task PersistInferenceOptions(InferenceOptions options, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Persist Llma Model Options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    Task PersistLlamaModelOptions(LlamaModelOptions options, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Inference Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<InferenceOptions?> GetInferenceOptions(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Llma Model Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LlamaModelOptions?> GetLlamaModelOptions(string userId, CancellationToken cancellationToken);
}
