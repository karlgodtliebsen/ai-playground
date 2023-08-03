using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Configuration;

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
    public static IServiceCollection AddLlamaConfiguration(this IServiceCollection services, LlamaModelOptions modelOptions)
    {
        VerifyOptions(modelOptions);
        services.AddSingleton<IOptions<LlamaModelOptions>>(new OptionsWrapper<LlamaModelOptions>(modelOptions));
        return services;
    }

    private static void VerifyOptions(LlamaModelOptions modelOptions)
    {
        ArgumentNullException.ThrowIfNull(modelOptions);
        ArgumentNullException.ThrowIfNull(modelOptions.ModelPath);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlamaConfiguration(this IServiceCollection services, Action<LlamaModelOptions>? options = null)
    {
        var modelOptions = new LlamaModelOptions();
        options?.Invoke(modelOptions);
        return services.AddLlamaConfiguration(modelOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlamaConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= LlamaModelOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<LlamaModelOptions>();
        if (modelOptions is null)
        {
            modelOptions = new LlamaModelOptions();
        }
        return services.AddLlamaConfiguration(modelOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlamaConfiguration(this IServiceCollection services, IConfiguration configuration, Action<LlamaModelOptions> options, string? sectionName = null)
    {
        sectionName ??= LlamaModelOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<LlamaModelOptions>();
        if (modelOptions is null)
        {
            modelOptions = new LlamaModelOptions();
        }
        options.Invoke(modelOptions);
        return services.AddLlamaConfiguration(modelOptions);
    }
}
