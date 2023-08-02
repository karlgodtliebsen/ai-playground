using System.Runtime.CompilerServices;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.Extensions.Options;

using OneOf;
using OneOf.Types;

using SerilogTimings.Extensions;

namespace LLamaSharpApp.WebAPI.Domain.Repositories.Implementation;


/// <summary>
/// Handles the users state
/// </summary>
public sealed class UsersStateFileRepository : IUsersStateRepository
{
    private readonly ILogger logger;
    private const string InferenceFile = "inference-options.json";
    private const string LlamaModelFile = "llamamodel-options.json";
    private const string AssetsPath = "Assets";
    private readonly WebApiOptions webApiOptions;
    private static readonly string FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));


    /// <summary>
    /// Constructor for the User State File Repository
    /// </summary>
    /// <param name="webApiOptions"></param>
    /// <param name="logger"></param>
    public UsersStateFileRepository(IOptions<WebApiOptions> webApiOptions, ILogger logger)
    {
        this.logger = logger;
        this.webApiOptions = webApiOptions.Value;
    }

    private async Task PersistObject(object options, string fileName, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("Saving Options");
        if (File.Exists(fileName)) File.Delete(fileName); //Can be improved to make it more resilient by creating a copy and so on
        await using var stream = File.OpenWrite(fileName);
        await JsonSerializer.SerializeAsync(stream, options, cancellationToken: cancellationToken);
        op.Complete();
    }

    private string GetFullPath(string userId)
    {
        var path = Path.GetFullPath(Path.Combine(FullPath, webApiOptions.StatePersistencePath, userId));
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

        using var op = logger.BeginOperation("Loading Inference Options for {userId}...", userId);
        await using var stream = File.OpenRead(fileName);
        var options = await JsonSerializer.DeserializeAsync<InferenceOptions>(stream, cancellationToken: cancellationToken);
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
        await using var stream = File.OpenRead(fileName);
        var options = await JsonSerializer.DeserializeAsync<LlamaModelOptions>(stream, cancellationToken: cancellationToken);
        op.Complete();
        return options!;
    }


    /// <inheritdoc />
    public async IAsyncEnumerable<string> GetSystemChatTemplates([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var directory = Path.Combine(FullPath, AssetsPath);
        foreach (var file in Directory.EnumerateFiles(directory, "*.txt"))
        {
            yield return await File.ReadAllTextAsync(file, cancellationToken);
        }
    }
}
