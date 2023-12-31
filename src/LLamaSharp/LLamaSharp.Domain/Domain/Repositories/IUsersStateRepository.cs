﻿using LLamaSharp.Domain.Configuration;

using OneOf;
using OneOf.Types;

namespace LLamaSharp.Domain.Domain.Repositories;

/// <summary>
/// Handles Users State for InferenceOptions and LlamaModelOptions
/// </summary>
public interface IUsersStateRepository
{
    /// <summary>
    /// Get the list of models
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetModels();

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
    Task PersistLlamaModelOptions(LLamaModelOptions options, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Inference Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<InferenceOptions, None>> GetInferenceOptions(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Get Llma Model Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<LLamaModelOptions, None>> GetLlamaModelOptions(string userId, CancellationToken cancellationToken);


    /// <summary>
    /// Returns the systems prompt templates from the assets files
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> GetSystemPromptTemplates(CancellationToken cancellationToken);

    /// <summary>
    /// Returns the specified system prompt templates from the assets files
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetSpecifiedSystemPromptTemplates(string name, string? version = null, CancellationToken? cancellationToken = null);
}
