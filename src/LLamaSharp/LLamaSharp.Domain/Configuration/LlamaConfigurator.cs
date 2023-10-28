using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LLamaSharp.Domain.Configuration;

/// <summary>
/// Llma model Configuration like ModelParams
/// </summary>
public static class LlamaConfigurator
{
    ///  <summary>
    ///  Model options
    /// https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
    ///  WizardLM  maps to wizardLM-7B.ggmlv3.q4_1.bin
    ///  </summary>
    ///  <param name="services"></param>
    ///  <param name="modelOptions"></param>
    ///  <returns></returns>
    public static IServiceCollection AddLLamaConfiguration(this IServiceCollection services, LLamaModelOptions modelOptions)
    {
        VerifyOptions(modelOptions);
        services.AddSingleton(Options.Create(modelOptions));
        return services;
    }

    private static void VerifyOptions(LLamaModelOptions modelOptions)
    {
        ArgumentNullException.ThrowIfNull(modelOptions);
        ArgumentNullException.ThrowIfNull(modelOptions.ModelPath);
    }

    /// <summary>
    /// Add programmatically customizable configuration for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddLLamaConfiguration(this IServiceCollection services, Action<LLamaModelOptions>? options = null)
    {
        var modelOptions = new LLamaModelOptions();
        options?.Invoke(modelOptions);
        return services.AddLLamaConfiguration(modelOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (LlamaModel) or the provided section name
    /// for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddLLamaConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= LLamaModelOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<LLamaModelOptions>();
        if (modelOptions is null)
        {
            modelOptions = new LLamaModelOptions();
        }
        return services.AddLLamaConfiguration(modelOptions);
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
    public static IServiceCollection AddLLamaConfiguration(this IServiceCollection services, IConfiguration configuration,
        Action<LLamaModelOptions> options, string? sectionName = null)
    {
        sectionName ??= LLamaModelOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<LLamaModelOptions>();
        if (modelOptions is null)
        {
            modelOptions = new LLamaModelOptions();
        }
        options.Invoke(modelOptions);
        return services.AddLLamaConfiguration(modelOptions);
    }
}
