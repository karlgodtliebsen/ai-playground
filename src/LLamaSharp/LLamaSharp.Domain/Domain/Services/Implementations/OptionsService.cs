using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using AI.Library.Utils;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Repositories;

using Microsoft.Extensions.Options;

namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class OptionsService : IOptionsService
{
    private readonly IUsersStateRepository stateRepository;
    private readonly ILogger logger;
    private readonly LLamaModelOptions lLamaModelOptions;
    private readonly InferenceOptions inferenceOptions;

    /// <summary>
    /// Constructor for the Options Service
    /// </summary>
    /// <param name="stateRepository"></param>
    /// <param name="inferenceOptions"></param>
    /// <param name="llmaOptions"></param>
    /// <param name="logger"></param>
    public OptionsService(
        IUsersStateRepository stateRepository,
        IOptions<InferenceOptions> inferenceOptions,
        IOptions<LLamaModelOptions> llmaOptions,
        ILogger logger)
    {
        this.stateRepository = stateRepository;
        this.logger = logger;
        lLamaModelOptions = llmaOptions.Value;
        this.inferenceOptions = inferenceOptions.Value;
    }

    /// <inheritdoc />
    public async Task PersistInferenceOptions(InferenceOptions? options, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then use default options
        if (options is null)
        {
            options = inferenceOptions.CreateSnapshot();
        }
        await stateRepository.PersistInferenceOptions(options!, userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PersistLlamaModelOptions(LLamaModelOptions? options, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then use default options
        if (options is null)
        {
            options = lLamaModelOptions.CreateSnapshot();
        }
        await stateRepository.PersistLlamaModelOptions(options!, userId, cancellationToken);
    }

    /// <summary>
    /// Creates a snapshot of the default LlamaModel options
    /// </summary>
    /// <returns></returns>
    public LLamaModelOptions GetDefaultLlamaModelOptions()
    {
        return lLamaModelOptions.CreateSnapshot();
    }

    /// <summary>
    /// Creates a snapshot of the default Inference options
    /// </summary>
    /// <returns></returns>
    public InferenceOptions GetDefaultInferenceOptions()
    {
        return inferenceOptions.CreateSnapshot();
    }

    /// <inheritdoc />
    public async Task<InferenceOptions> CoalsceInferenceOptions(InferenceOptions? queryOptions, string userId, CancellationToken cancellationToken)
    {
        if (queryOptions is not null)
        {
            return queryOptions;
        }
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetInferenceOptions(userId, cancellationToken))
        .Match(
             options => options,
             _ => inferenceOptions.CreateSnapshot()
            );
    }

    /// <inheritdoc />
    public async Task<LLamaModelOptions> CoalsceLlamaModelOptions(LLamaModelOptions? queryOptions, string userId, CancellationToken cancellationToken)
    {
        if (queryOptions is not null)
        {
            return queryOptions;
        }
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetLlamaModelOptions(userId, cancellationToken))
            .Match(
                options => options,
                _ => lLamaModelOptions.CreateSnapshot()
            );
    }

    public IEnumerable<string> GetModels()
    {
        var models = stateRepository.GetModels();
        foreach (var model in models)
        {
            yield return model;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> GetSystemPromptTemplates([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var templates = stateRepository.GetSystemPromptTemplates(cancellationToken);
        await foreach (var template in templates.WithCancellation(cancellationToken))
        {
            yield return template;
        }
    }
    public async Task<string> GetSpecifiedSystemPromptTemplates(string name, string? version = null, CancellationToken? cancellationToken = null)
    {
        return await stateRepository.GetSpecifiedSystemPromptTemplates(name, version, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InferenceOptions> GetInferenceOptions(string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetInferenceOptions(userId, cancellationToken))
            .Match(
                options => options,
                _ => inferenceOptions.CreateSnapshot()
            );
    }

    /// <inheritdoc />
    public async Task<LLamaModelOptions> GetLlamaModelOptions(string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetLlamaModelOptions(userId, cancellationToken))
            .Match(
                options => options,
                _ => lLamaModelOptions.CreateSnapshot()
            );
    }
}

internal static class JsonExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    public static InferenceOptions CreateSnapshot(this InferenceOptions inferenceOptions)
    {
        return inferenceOptions.ToJson(SerializerOptions).FromJson<InferenceOptions>(SerializerOptions)!;
    }
    public static LLamaModelOptions CreateSnapshot(this LLamaModelOptions lLamaModelOptions)
    {
        return lLamaModelOptions.ToJson(SerializerOptions).FromJson<LLamaModelOptions>(SerializerOptions)!;
    }
}
