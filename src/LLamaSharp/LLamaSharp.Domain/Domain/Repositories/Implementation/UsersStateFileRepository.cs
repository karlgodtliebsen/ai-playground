﻿using System.Runtime.CompilerServices;

using AI.Library.Utils;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Options;

using OneOf;
using OneOf.Types;

using SerilogTimings.Extensions;

namespace LLamaSharp.Domain.Domain.Repositories.Implementation;


/// <summary>
/// Handles the users state
/// </summary>
public sealed class UsersStateFileRepository : IUsersStateRepository
{
    private readonly ILogger logger;
    private const string InferenceFile = "inference-options.json";
    private const string LlamaModelFile = "llamamodel-options.json";
    private const string AssetsPath = "Assets";
    private readonly LlamaRepositoryOptions llamaRepositoryOptions;
    private static readonly string FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

    /// <summary>
    /// Constructor for the User State File Repository
    /// </summary>
    /// <param name="webApiOptions"></param>
    /// <param name="logger"></param>
    public UsersStateFileRepository(IOptions<LlamaRepositoryOptions> webApiOptions, ILogger logger)
    {
        this.logger = logger;
        this.llamaRepositoryOptions = webApiOptions.Value;
    }

    private async Task PersistObject(object obj, string fileName, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("Saving Options");
        FileInfo fileInfo = new FileInfo(fileName);
        await obj.ToJsonFile(fileInfo, cancellationToken);
        op.Complete();
    }

    private string GetFullPath(string userId)
    {
        var path = Path.GetFullPath(Path.Combine(FullPath, llamaRepositoryOptions.StatePersistencePath, userId));
        Directory.CreateDirectory(path!);
        return path;
    }
    private string GetFileName(string userId, string fileName)
    {
        var path = GetFullPath(userId);
        return Path.Combine(path, fileName);
    }

    /// <summary>
    /// Persist Inference Options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    public async Task PersistInferenceOptions(InferenceOptions options, string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, InferenceFile);
        await PersistObject(options, fileName, cancellationToken);
    }

    /// <summary>
    /// Persist Llma Model Options
    /// </summary>
    /// <param name="options"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    public async Task PersistLlamaModelOptions(LlamaModelOptions options, string userId, CancellationToken cancellationToken)
    {
        options.TensorSplits = IntPtr.Zero;
        var fileName = GetFileName(userId, LlamaModelFile);
        await PersistObject(options, fileName, cancellationToken);
    }

    /// <summary>
    /// Get Inference Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<InferenceOptions, None>> GetInferenceOptions(string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, InferenceFile);
        if (!File.Exists(fileName)) return new None();

        FileInfo fileInfo = new FileInfo(fileName);
        using var op = logger.BeginOperation("Loading Inference Options for {userId}...", userId);
        var options = await fileInfo.FromJsonFile<InferenceOptions>(cancellationToken);
        op.Complete();
        return options!;
    }

    /// <summary>
    /// Get Llma Model Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<LlamaModelOptions, None>> GetLlamaModelOptions(string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, LlamaModelFile);
        if (!File.Exists(fileName)) return new None();
        using var op = logger.BeginOperation("Loading Llama Model Options for {userId}...", userId);
        FileInfo fileInfo = new FileInfo(fileName);
        var options = await fileInfo.FromJsonFile<LlamaModelOptions>(cancellationToken);
        op.Complete();
        return options!;
    }


    /// <inheritdoc />
    public async IAsyncEnumerable<string> GetSystemPromptTemplates([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var directory = Path.Combine(FullPath, AssetsPath, "prompts");
        foreach (var file in Directory.EnumerateFiles(directory, "*-prompt.txt"))
        {
            yield return await File.ReadAllTextAsync(file, cancellationToken);
        }
    }
}
