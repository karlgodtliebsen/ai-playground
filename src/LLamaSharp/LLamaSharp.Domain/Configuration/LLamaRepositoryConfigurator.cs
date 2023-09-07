using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Repositories.Implementation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LLamaSharp.Domain.Configuration;

/// <summary>
/// LLama Repository Configuration 
/// </summary>
public static class LLamaRepositoryConfigurator
{

    ///  <summary>
    ///  Model options
    /// https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
    ///  </summary>
    ///  <param name="services"></param>
    ///  <param name="modelOptions"></param>
    ///  <returns></returns>
    public static IServiceCollection AddLLamaRepository(this IServiceCollection services, LlamaRepositoryOptions modelOptions)
    {
        VerifyOptions(modelOptions);
        services.AddSingleton<IOptions<LlamaRepositoryOptions>>(new OptionsWrapper<LlamaRepositoryOptions>(modelOptions));
        services
            .AddTransient<IContextStateRepository, ContextStateFileRepository>()
            .AddTransient<IUsersStateRepository, UsersStateFileRepository>();
        return services;
    }

    private static void VerifyOptions(LlamaRepositoryOptions modelOptions)
    {
        ArgumentNullException.ThrowIfNull(modelOptions);
        ArgumentNullException.ThrowIfNull(modelOptions.ModelStatePersistencePath);
        ArgumentNullException.ThrowIfNull(modelOptions.StatePersistencePath);
    }

    /// <summary>
    /// Add programmatically customizable configuration for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddLLamaRepository(this IServiceCollection services, Action<LlamaRepositoryOptions>? options = null)
    {
        var modelOptions = new LlamaRepositoryOptions();
        options?.Invoke(modelOptions);
        return services.AddLLamaRepository(modelOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (LlamaModel) or the provided section name
    /// for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddLLamaRepository(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= LLamaModelOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<LlamaRepositoryOptions>() ?? new LlamaRepositoryOptions();
        return services.AddLLamaRepository(modelOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (LlamaModel) or the provided section name
    /// and supports programmatically customizable configuration options for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddLLamaRepository(this IServiceCollection services, IConfiguration configuration, Action<LlamaRepositoryOptions> options, string? sectionName = null)
    {
        sectionName ??= LlamaRepositoryOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<LlamaRepositoryOptions>() ?? new LlamaRepositoryOptions();
        options.Invoke(modelOptions);
        return services.AddLLamaRepository(modelOptions);
    }

}
