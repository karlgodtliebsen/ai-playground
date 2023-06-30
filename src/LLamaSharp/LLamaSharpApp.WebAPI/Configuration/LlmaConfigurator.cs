using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// Llma model Configuration like ModelParams and InferenceOptions
/// </summary>
public static class LlmaConfigurator
{
    ///  <summary>
    ///  Model options
    /// https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
    ///  WizardLM  maps to wizardLM-7B.ggmlv3.q4_1.bin
    ///  </summary>
    ///  <param name="services"></param>
    ///  <param name="llmaModelOptions"></param>
    ///  <param name="inferenceOptions"></param>
    ///  <returns></returns>
    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, LlmaModelOptions llmaModelOptions, InferenceOptions inferenceOptions)
    {
        VerifyAndAddOptions(services, llmaModelOptions);
        VerifyAndAddOptions(services, inferenceOptions);
        return services;
    }

    private static void VerifyAndAddOptions(IServiceCollection services, LlmaModelOptions modelOptions)
    {
        ArgumentNullException.ThrowIfNull(modelOptions);
        ArgumentNullException.ThrowIfNull(modelOptions.ModelPath);
        services.AddSingleton<IOptions<LlmaModelOptions>>(new OptionsWrapper<LlmaModelOptions>(modelOptions));
    }

    private static void VerifyAndAddOptions(IServiceCollection services, InferenceOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton<IOptions<InferenceOptions>>(new OptionsWrapper<InferenceOptions>(options));
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="llmaOptions"></param>
    /// <param name="inferenceOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services,
        Action<LlmaModelOptions>? llmaOptions = null,
        Action<InferenceOptions>? inferenceOptions = null)
    {
        var configuredLlmaOptions = new LlmaModelOptions();
        llmaOptions?.Invoke(configuredLlmaOptions);
        var configuredInferenceOptions = new InferenceOptions();
        inferenceOptions?.Invoke(configuredInferenceOptions);
        return services.AddLlmaConfiguration(configuredLlmaOptions, configuredInferenceOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="llmaOptionsSectionName"></param>
    /// <param name="inferenceOptionsSectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services,
        IConfiguration configuration,
        string? llmaOptionsSectionName = null,
        string? inferenceOptionsSectionName = null
        )
    {
        if (llmaOptionsSectionName is null)
        {
            llmaOptionsSectionName = LlmaModelOptions.SectionName;
        }
        if (inferenceOptionsSectionName is null)
        {
            inferenceOptionsSectionName = InferenceOptions.SectionName;
        }
        var configuredLlmaOptions = configuration.GetSection(llmaOptionsSectionName).Get<LlmaModelOptions>()!;
        var configuredInferenceOptions = configuration.GetSection(inferenceOptionsSectionName).Get<InferenceOptions>()!;
        if (configuredLlmaOptions is null) configuredLlmaOptions = new LlmaModelOptions();
        if (configuredInferenceOptions is null) configuredInferenceOptions = new InferenceOptions();
        return services.AddLlmaConfiguration(configuredLlmaOptions, configuredInferenceOptions);
    }
}
