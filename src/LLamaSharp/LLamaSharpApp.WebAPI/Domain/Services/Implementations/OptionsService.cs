using System.Text.Json;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Repositories;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class OptionsService : IOptionsService
{
    private readonly IUsersStateRepository stateRepository;
    private readonly ILogger<OptionsService> logger;
    private readonly LlamaModelOptions llamaModelOptions;
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
        IOptions<LlamaModelOptions> llmaOptions,
        ILogger<OptionsService> logger)
    {
        this.stateRepository = stateRepository;
        this.logger = logger;
        llamaModelOptions = llmaOptions.Value;
        this.inferenceOptions = inferenceOptions.Value;
    }

    /// <inheritdoc />
    public async Task PersistInferenceOptions(InferenceOptions? options, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist  then use default options
        if (options is null)
        {
            options = inferenceOptions.ToJson().FromJson<InferenceOptions>();
        }
        await stateRepository.PersistInferenceOptions(options!, userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PersistLlamaModelOptions(LlamaModelOptions? options, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then use default options
        if (options is null)
        {
            options = llamaModelOptions.ToJson().FromJson<LlamaModelOptions>();
        }
        await stateRepository.PersistLlamaModelOptions(options!, userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InferenceOptions> CoalsceInferenceOptions(InferenceOptions? queryOptions, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        if (queryOptions is not null)
        {
            return queryOptions;
        }

        var options = await stateRepository.GetInferenceOptions(userId, cancellationToken);
        if (options is null)
        {
            options = inferenceOptions.ToJson().FromJson<InferenceOptions>();
        }
        return options!;
    }

    /// <inheritdoc />
    public async Task<LlamaModelOptions> CoalsceLlamaModelOptions(LlamaModelOptions? queryOptions, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        if (queryOptions is not null)
        {
            return queryOptions;
        }
        //if options for the user does not exist then return snapshot of default options
        var options = await stateRepository.GetLlamaModelOptions(userId, cancellationToken);
        if (options is null)
        {
            options = llamaModelOptions.ToJson().FromJson<LlamaModelOptions>();
        }
        return options!;
    }

    /// <inheritdoc />
    public async Task<InferenceOptions> GetInferenceOptions(string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        var options = await stateRepository.GetInferenceOptions(userId, cancellationToken);
        if (options is null)
        {
            options = inferenceOptions.ToJson().FromJson<InferenceOptions>();//snapshot
        }
        return options!;
    }

    /// <inheritdoc />
    public async Task<LlamaModelOptions> GetLlamaModelOptions(string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        var options = await stateRepository.GetLlamaModelOptions(userId, cancellationToken);
        if (options is null)
        {
            options = llamaModelOptions.ToJson().FromJson<LlamaModelOptions>();//snapshot
        }
        return options!;
    }
}

internal static class JsonExtensions
{

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string ToJson(this object obj, JsonSerializerOptions? serializerOptions = null)
    {
        if (serializerOptions is not null)
        {
            return JsonSerializer.Serialize(obj, serializerOptions!);
        }
        return JsonSerializer.Serialize(obj, JsonOptions);
    }

    public static string AsJson(this object obj, JsonSerializerOptions? serializerOptions = null)
    {
        return ToJson(obj, serializerOptions);
    }

    public static T? FromJson<T>(this string json, JsonSerializerOptions? serializerOptions = null)
    {
        if (serializerOptions is not null)
        {
            return JsonSerializer.Deserialize<T>(json, serializerOptions);
        }
        return JsonSerializer.Deserialize<T>(json);
    }
}
