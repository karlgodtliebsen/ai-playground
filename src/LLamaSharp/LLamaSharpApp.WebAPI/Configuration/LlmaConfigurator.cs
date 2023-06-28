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
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, LlmaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        //validate properties of LlmaOptions
        ArgumentNullException.ThrowIfNull(options.Model);
        ArgumentNullException.ThrowIfNull(options.ContextSize);
        ArgumentNullException.ThrowIfNull(options.GpuLayerCount);
        ArgumentNullException.ThrowIfNull(options.Seed);
        ArgumentNullException.ThrowIfNull(options.UseFp16Memory);
        ArgumentNullException.ThrowIfNull(options.UseMemorymap);
        ArgumentNullException.ThrowIfNull(options.UseMemoryLock);
        ArgumentNullException.ThrowIfNull(options.Perplexity);
        ArgumentNullException.ThrowIfNull(options.LoraAdapter);
        ArgumentNullException.ThrowIfNull(options.LoraBase);
        ArgumentNullException.ThrowIfNull(options.Threads);
        ArgumentNullException.ThrowIfNull(options.BatchSize);
        ArgumentNullException.ThrowIfNull(options.ConvertEosToNewLine);
        ArgumentNullException.ThrowIfNull(options.EmbeddingMode);


        services.AddSingleton<IOptions<LlmaOptions>>(new OptionsWrapper<LlmaOptions>(options));
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<IEmbeddingsService, EmbeddingsService>();
        services.AddTransient<ILlmaModelFactory, LlmaModelFactory>();
        return services;
    }


    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, Action<LlmaOptions>? options = null)
    {
        var configuredOptions = new LlmaOptions();
        options?.Invoke(configuredOptions);
        return services.AddLlmaConfiguration(configuredOptions);
    }

    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = LlmaOptions.SectionName;
        }
        services.Configure<LlmaOptions>(configuration.GetSection(sectionName));
        var configuredOptions = configuration.GetSection(sectionName).Get<LlmaOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddLlmaConfiguration(configuredOptions);
    }
}
