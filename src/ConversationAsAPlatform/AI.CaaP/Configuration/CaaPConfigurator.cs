using AI.CaaP.AgentsDomain;
using AI.CaaP.Domain;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;

namespace AI.CaaP.Configuration;

public static class CaaPConfigurator
{

    public static IServiceCollection AddCaaP(this IServiceCollection services, CaaPOptions options, ChunkOptions chunkOptions, OpenAIOptions aiOptions)
    {
        services
            .AddSingleton<IOptions<CaaPOptions>>(new OptionsWrapper<CaaPOptions>(options))
            .AddSingleton<IOptions<ChunkOptions>>(new OptionsWrapper<ChunkOptions>(chunkOptions))
            .AddTransient<IAgentService, AgentService>()
            .AddTransient<ILanguageService, LanguageService>()
            .AddTransient<IConversationService, ConversationService>()
            .AddTransient<ITextChopperService, TextChopperService>();

        services.AddOpenAIConfiguration(aiOptions);
        return services;
    }

    public static IServiceCollection AddCaaP(this IServiceCollection services,
                        Action<CaaPOptions>? options = null,
                        Action<ChunkOptions>? chunkOptions = null,
                        Action<OpenAIOptions>? aiOptions = null
        )
    {
        var configuredOptions = new CaaPOptions();
        options?.Invoke(configuredOptions);

        var configuredChunkOptions = new ChunkOptions();
        chunkOptions?.Invoke(configuredChunkOptions);

        var configuredAiOptions = new OpenAIOptions();
        aiOptions?.Invoke(configuredAiOptions);

        return services.AddCaaP(configuredOptions, configuredChunkOptions, configuredAiOptions);
    }

    public static IServiceCollection AddCaaP(this IServiceCollection services, IConfiguration configuration,
                            string? sectionName = null,
                            string? chunkSectionName = null,
                            string? aiOptionsSectionName = null
        )
    {
        if (sectionName is null)
        {
            sectionName = CaaPOptions.ConfigSectionName;
        }
        if (chunkSectionName is null)
        {
            chunkSectionName = ChunkOptions.ConfigSectionName;
        }

        if (aiOptionsSectionName is null)
        {
            aiOptionsSectionName = OpenAIOptions.ConfigSectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<CaaPOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        var configuredChunkOptions = configuration.GetSection(chunkSectionName).Get<ChunkOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredChunkOptions);
        var configuredAiOptions = configuration.GetSection(aiOptionsSectionName).Get<OpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredAiOptions);
        return services.AddCaaP(configuredOptions, configuredChunkOptions, configuredAiOptions);
    }

}
