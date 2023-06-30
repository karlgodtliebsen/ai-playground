using LLamaSharpApp.WebAPI.Services;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// Llma Configuration 
/// </summary>
public static class LlmaConfigurator
{
    /// <summary>
    /// Model options
    ///https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
    /// WizardLM  maps to wizardLM-7B.ggmlv3.q4_1.bin
    /// </summary>
    /// <param name="services"></param>
    /// <param name="llmaOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, LlmaOptions llmaOptions, InferenceOptions inferenceOptions)
    {
        VerifyAndAddOptions(services, llmaOptions);
        VerifyAndAddOptions(services, inferenceOptions);
        AddDomain(services);
        return services;
    }

    private static void VerifyAndAddOptions(IServiceCollection services, LlmaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        //validate properties of LlmaOptions
        ArgumentNullException.ThrowIfNull(options.ModelPath);
        services.AddSingleton<IOptions<LlmaOptions>>(new OptionsWrapper<LlmaOptions>(options)); //TODO: replace or extend options wrapper to support reload
    }

    private static void VerifyAndAddOptions(IServiceCollection services, InferenceOptions options)
    {
        //ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton<IOptions<InferenceOptions>>(new OptionsWrapper<InferenceOptions>(options));   //TODO: replace or extend options wrapper to support reload
    }

    private static void AddDomain(IServiceCollection services)
    {
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<IEmbeddingsService, EmbeddingsService>();
        services.AddTransient<IExecutorService, ExecutorService>();
        services.AddTransient<ITokenizationService, TokenizationService>();
        services.AddTransient<ILlmaModelFactory, LlmaModelFactory>();
        services.AddTransient<IStateHandler, StateHandler>();

    }


    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, Action<LlmaOptions>? llmaOptions = null, Action<InferenceOptions>? inferenceOptions = null)
    {
        var configuredLlmaOptions = new LlmaOptions();
        llmaOptions?.Invoke(configuredLlmaOptions);
        var configuredInferenceOptions = new InferenceOptions();
        inferenceOptions?.Invoke(configuredInferenceOptions);
        return services.AddLlmaConfiguration(configuredLlmaOptions, configuredInferenceOptions);
    }

    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, IConfiguration configuration, string? llmaOptionsSectionName = null, string? inferenceOptionsSectionName = null)
    {
        if (llmaOptionsSectionName is null)
        {
            llmaOptionsSectionName = LlmaOptions.SectionName;
        }
        if (inferenceOptionsSectionName is null)
        {
            inferenceOptionsSectionName = InferenceOptions.SectionName;
        }
        //services.Configure<LlmaOptions>(configuration.GetSection(llmaOptionsSectionName));
        var configuredLlmaOptions = configuration.GetSection(llmaOptionsSectionName).Get<LlmaOptions>()!;

        //services.Configure<InferenceOptions>(configuration.GetSection(inferenceOptionsSectionName));
        var configuredInferenceOptions = configuration.GetSection(inferenceOptionsSectionName).Get<InferenceOptions>()!;

        return services.AddLlmaConfiguration(configuredLlmaOptions, configuredInferenceOptions);
    }
}
