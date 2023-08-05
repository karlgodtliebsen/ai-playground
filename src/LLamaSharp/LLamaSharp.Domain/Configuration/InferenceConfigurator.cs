using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LLamaSharp.Domain.Configuration;

/// <summary>
/// LLama Inference model Configuration
/// </summary>
public static class InferenceConfigurator
{
    ///  <summary>
    ///  Inference Model options
    /// https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
    ///  WizardLM  maps to wizardLM-7B.ggmlv3.q4_1.bin
    ///  </summary>
    ///  <param name="services"></param>
    ///  <param name="inferenceOptions"></param>
    ///  <returns></returns>
    public static IServiceCollection AddInferenceConfiguration(this IServiceCollection services, InferenceOptions inferenceOptions)
    {
        VerifyOptions(inferenceOptions);
        services.AddSingleton<IOptions<InferenceOptions>>(new OptionsWrapper<InferenceOptions>(inferenceOptions));
        return services;
    }
    private static void VerifyOptions(InferenceOptions inferenceOptions)
    {
        ArgumentNullException.ThrowIfNull(inferenceOptions);
    }

    /// <summary>
    /// Add programmatically customizable configuration for the Inference Options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddInferenceConfiguration(this IServiceCollection services, Action<InferenceOptions>? options = null)
    {
        var inferenceOptions = new InferenceOptions();
        options?.Invoke(inferenceOptions);
        return services.AddInferenceConfiguration(inferenceOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (inference) or the provided section name
    /// for the Inference Options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddInferenceConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= InferenceOptions.SectionName;
        var options = configuration.GetSection(sectionName).Get<InferenceOptions>();
        if (options is null)
        {
            options = new InferenceOptions();
        }
        return services.AddInferenceConfiguration(options);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (inference) or the provided section name
    /// and add programmatically customizable configuration for the Inference Options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddInferenceConfiguration(this IServiceCollection services, IConfiguration configuration,
        Action<InferenceOptions> options, string? sectionName = null)
    {
        sectionName ??= InferenceOptions.SectionName;
        var inferenceOptions = configuration.GetSection(sectionName).Get<InferenceOptions>();
        if (inferenceOptions is null)
        {
            inferenceOptions = new InferenceOptions();
        }
        options.Invoke(inferenceOptions);
        return services.AddInferenceConfiguration(inferenceOptions);
    }

}
