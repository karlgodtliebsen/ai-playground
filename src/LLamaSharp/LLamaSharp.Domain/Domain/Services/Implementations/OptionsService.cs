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
    private readonly LlamaModelOptions llamaModelOptions;
    private readonly InferenceOptions inferenceOptions;
    private readonly JsonSerializerOptions serializerOptions;
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
        ILogger logger)
    {
        this.stateRepository = stateRepository;
        this.logger = logger;
        llamaModelOptions = llmaOptions.Value;
        this.inferenceOptions = inferenceOptions.Value;
        serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
    }

    /// <inheritdoc />
    public async Task PersistInferenceOptions(InferenceOptions? options, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then use default options
        if (options is null)
        {
            options = inferenceOptions.CreateSnapshot(serializerOptions);
        }
        await stateRepository.PersistInferenceOptions(options!, userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PersistLlamaModelOptions(LlamaModelOptions? options, string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then use default options
        if (options is null)
        {
            options = llamaModelOptions.CreateSnapshot(serializerOptions);
        }
        await stateRepository.PersistLlamaModelOptions(options!, userId, cancellationToken);
    }

    /// <summary>
    /// Creates a snapshot of the default LlamaModel options
    /// </summary>
    /// <returns></returns>
    public LlamaModelOptions GetDefaultLlamaModelOptions()
    {
        return llamaModelOptions.CreateSnapshot(serializerOptions);
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
             _ => inferenceOptions.CreateSnapshot(serializerOptions)
            );
    }

    /// <inheritdoc />
    public async Task<LlamaModelOptions> CoalsceLlamaModelOptions(LlamaModelOptions? queryOptions, string userId, CancellationToken cancellationToken)
    {
        if (queryOptions is not null)
        {
            return queryOptions;
        }
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetLlamaModelOptions(userId, cancellationToken))
            .Match(
                options => options,
                _ => llamaModelOptions.CreateSnapshot(serializerOptions)
            );
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

    /// <inheritdoc />
    public async Task<InferenceOptions> GetInferenceOptions(string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetInferenceOptions(userId, cancellationToken))
            .Match(
                options => options,
                _ => inferenceOptions.CreateSnapshot(serializerOptions)
            );
    }

    /// <inheritdoc />
    public async Task<LlamaModelOptions> GetLlamaModelOptions(string userId, CancellationToken cancellationToken)
    {
        //if options for the user does not exist then return snapshot of default options
        return (await stateRepository.GetLlamaModelOptions(userId, cancellationToken))
            .Match(
                options => options,
                _ => llamaModelOptions.CreateSnapshot(serializerOptions)
            );
    }
}

internal static class JsonExtensions
{
    public static InferenceOptions CreateSnapshot(this InferenceOptions inferenceOptions, JsonSerializerOptions serializerOptions)
    {
        return inferenceOptions.ToJson(serializerOptions).FromJson<InferenceOptions>(serializerOptions)!;
    }
    public static LlamaModelOptions CreateSnapshot(this LlamaModelOptions llamaModelOptions, JsonSerializerOptions serializerOptions)
    {
        return llamaModelOptions.ToJson(serializerOptions).FromJson<LlamaModelOptions>(serializerOptions)!;
    }
}
