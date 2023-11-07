using AI.CaaP.AgentsDomain;
using AI.CaaP.Domain;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;

namespace AI.CaaP.Configuration;

public static class CaaPConfigurator
{

    public static IServiceCollection AddCaaP(this IServiceCollection services, CaaPOptions options, ChunkOptions chunkOptions, OpenAIConfiguration aiConfiguration)
    {
        services
            .AddSingleton<IOptions<CaaPOptions>>(Options.Create(options))
            .AddSingleton<IOptions<ChunkOptions>>(Options.Create(chunkOptions))
            .AddTransient<IAgentService, AgentService>()
            .AddTransient<ILanguageService, LanguageService>()
            .AddTransient<IConversationService, ConversationService>()
            .AddTransient<ITextChopperService, TextChopperService>();

        services.AddOpenAIConfiguration(aiConfiguration);
        return services;
    }

    public static IServiceCollection AddCaaP(this IServiceCollection services,
                        Action<CaaPOptions>? options = null,
                        Action<ChunkOptions>? chunkOptions = null,
                        Action<OpenAIConfiguration>? aiOptions = null
        )
    {
        var configuredOptions = new CaaPOptions();
        options?.Invoke(configuredOptions);

        var configuredChunkOptions = new ChunkOptions();
        chunkOptions?.Invoke(configuredChunkOptions);

        var configuredAiOptions = new OpenAIConfiguration();
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
            aiOptionsSectionName = OpenAIConfiguration.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<CaaPOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        var configuredChunkOptions = configuration.GetSection(chunkSectionName).Get<ChunkOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredChunkOptions);
        var configuredAiOptions = configuration.GetSection(aiOptionsSectionName).Get<OpenAIConfiguration>()!;
        ArgumentNullException.ThrowIfNull(configuredAiOptions);
        return services.AddCaaP(configuredOptions, configuredChunkOptions, configuredAiOptions);
    }

}
