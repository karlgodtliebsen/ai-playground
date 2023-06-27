using LLama.OldVersion;

using LLamaSharpApp.WebAPI.Services;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Configuration;

public static class LlmaConfigurator
{


    /// <summary>
    /// Models
    ///https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
    /// WizardLM  maps to wizardLM-7B.ggmlv3.q4_1.bin
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddLlmaConfiguration(this IServiceCollection services, LlmaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        LLamaModel model = new(new LLamaParams(
            model: options.Model,
            n_ctx: options.MaximumNumberOfTokens,
            interactive: options.Interactive,
            repeat_penalty: options.RepeatPenalty,
            verbose_prompt: options.VerbosePrompt)
        );
        services.AddSingleton<IOptions<LLamaModel>>(new OptionsWrapper<LLamaModel>(model));
        services.AddSingleton<IOptions<LlmaOptions>>(new OptionsWrapper<LlmaOptions>(options));
        services.AddTransient<IChatService, ChatService>();
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
            sectionName = LlmaOptions.ConfigSectionName;
        }
        services.Configure<LlmaOptions>(configuration.GetSection(sectionName));
        var configuredOptions = configuration.GetSection(sectionName).Get<LlmaOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddLlmaConfiguration(configuredOptions);
    }
}
