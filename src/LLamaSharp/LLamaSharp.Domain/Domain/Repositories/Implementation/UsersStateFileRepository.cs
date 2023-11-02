using System.Runtime.CompilerServices;

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
    private readonly LLamaModelOptions modelOptions;
    private readonly ILogger logger;
    private const string InferenceFile = "inference-options.json";
    private const string LlamaModelFile = "llamamodel-options.json";
    private const string AssetsPath = "Assets";
    private readonly LlamaRepositoryOptions repositoryOptions;
    private static readonly string FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

    /// <summary>
    /// Constructor for the User State File Repository
    /// </summary>
    /// <param name="repositoryOptions"></param>
    /// <param name="modelOptions"></param>
    /// <param name="logger"></param>
    public UsersStateFileRepository(IOptions<LlamaRepositoryOptions> repositoryOptions, IOptions<LLamaModelOptions> modelOptions, ILogger logger)
    {
        this.modelOptions = modelOptions.Value;
        this.repositoryOptions = repositoryOptions.Value;
        this.logger = logger;
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
        var path = Path.GetFullPath(Path.Combine(FullPath, repositoryOptions.StatePersistencePath, userId));
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
    public async Task PersistLlamaModelOptions(LLamaModelOptions options, string userId, CancellationToken cancellationToken)
    {
        //options.TensorSplits = Array.Empty<float>();
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
    public async Task<OneOf<LLamaModelOptions, None>> GetLlamaModelOptions(string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, LlamaModelFile);
        if (!File.Exists(fileName)) return new None();
        using var op = logger.BeginOperation("Loading Llama Model Options for {userId}...", userId);
        FileInfo fileInfo = new FileInfo(fileName);
        var options = await fileInfo.FromJsonFile<LLamaModelOptions>(cancellationToken);
        op.Complete();
        return options!;
    }


    /// <inheritdoc />
    public async IAsyncEnumerable<string> GetSystemPromptTemplates([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var directory = Path.GetFullPath(Path.Combine(FullPath, AssetsPath, "prompts"));
        foreach (var file in Directory.EnumerateFiles(directory, "*-prompt*.txt"))
        {
            yield return await File.ReadAllTextAsync(file, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<string> GetSpecifiedSystemPromptTemplates(string name, string? version = null, CancellationToken? cancellationToken = null)
    {
        var directory = Path.GetFullPath(Path.Combine(FullPath, AssetsPath, "prompts"));
        if (!string.IsNullOrEmpty(version))
        {
            version = "-" + version;
        }
        var file = Directory.EnumerateFiles(directory, $"{name}-prompt{version}.txt").FirstOrDefault();
        if (file is null)
        {
            throw new FileNotFoundException($"Prompt file {name}-prompt{version}.txt not found");
        }
        cancellationToken ??= CancellationToken.None;
        return await File.ReadAllTextAsync(file, cancellationToken.Value);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetModels()
    {
        var directory = Path.GetFullPath(Path.GetDirectoryName(modelOptions.ModelPath)!);
        foreach (var file in Directory.EnumerateFiles(directory, "*.gguf"))
        {

            yield return Path.GetFileNameWithoutExtension(file);
        }
    }
}
