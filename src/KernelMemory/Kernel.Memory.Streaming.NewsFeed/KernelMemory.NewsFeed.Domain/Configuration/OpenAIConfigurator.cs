using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public static class OpenAIConfigurator
{
    /// <summary>
    /// AddOpenAIConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, OpenAIOptions options)
    {
        Log.Logger.Information("Setting up OpenAI Configuration");
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(Options.Create(options));
        Log.Logger.Information("Completed Adding OpenAI Configuration");
        return services;
    }


    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, Action<OpenAIOptions>? options = null)
    {
        var configuredOptions = new OpenAIOptions();
        options?.Invoke(configuredOptions);
        return services.AddOpenAIConfiguration(configuredOptions);
    }

    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = OpenAIOptions.ConfigSectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddOpenAIConfiguration(configuredOptions);
    }
}
