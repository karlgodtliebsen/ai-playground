using System.Text.Json;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Repositories.Implementation;



/// <summary>
/// Handles the users state
/// </summary>
public class UsersStateFileRepository : IUsersStateRepository
{
    private const string InferenceFile = "inference-options.json";
    private const string LlmaModelFile = "llmamodel-options.json";
    private readonly WebApiOptions webApiOptions;
    private readonly string fullPath;

    /// <summary>
    /// Constructor for the User State File Repository
    /// </summary>
    /// <param name="webApiOptions"></param>
    public UsersStateFileRepository(IOptions<WebApiOptions> webApiOptions)
    {
        this.webApiOptions = webApiOptions.Value;
        fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
    }

    private async Task PersistObject(object options, string fileName, CancellationToken cancellationToken)
    {
        if (File.Exists(fileName)) File.Delete(fileName); //Can be improved to make it more resilient by creating a copy and so on
        await using var stream = File.OpenWrite(fileName);
        await JsonSerializer.SerializeAsync(stream, options, cancellationToken: cancellationToken);
    }

    private string GetFullPath(string userId)
    {
        var path = Path.GetFullPath(Path.Combine(fullPath, webApiOptions.StatePersistencePath, userId));
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
    public async Task PersistLlmaModelOptions(LlmaModelOptions options, string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, LlmaModelFile);
        await PersistObject(options, fileName, cancellationToken);
    }

    /// <summary>
    /// Get Inference Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<InferenceOptions?> GetInferenceOptions(string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, InferenceFile);
        if (!File.Exists(fileName)) return null;
        await using var stream = File.OpenRead(fileName);
        var options = await JsonSerializer.DeserializeAsync<InferenceOptions>(stream, cancellationToken: cancellationToken);
        return options;
    }

    /// <summary>
    /// Get Llma Model Options
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<LlmaModelOptions?> GetLlmaModelOptions(string userId, CancellationToken cancellationToken)
    {
        var fileName = GetFileName(userId, LlmaModelFile);
        if (!File.Exists(fileName)) return null;
        await using var stream = File.OpenRead(fileName);
        var options = await JsonSerializer.DeserializeAsync<LlmaModelOptions>(stream, cancellationToken: cancellationToken);
        return options;
    }
}
