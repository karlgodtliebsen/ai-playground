using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SemanticKernel.Tests.Configuration;

public static class AzureOpenAIConfigurator
{

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, AzureOpenAIOptions options)
    {
        services.AddSingleton<IOptions<AzureOpenAIOptions>>(Options.Create(options));
        return services;
    }

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, Action<AzureOpenAIOptions>? options = null)
    {
        var configuredOptions = new AzureOpenAIOptions();
        options?.Invoke(configuredOptions);
        return services.AddAzureOpenAI(configuredOptions);
    }

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= AzureOpenAIOptions.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<AzureOpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddAzureOpenAI(configuredOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (Azure) or the provided section name
    /// and add programmatically customizable configuration for the Inference Options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, IConfiguration configuration, Action<AzureOpenAIOptions> options, string? sectionName = null)
    {
        sectionName ??= AzureOpenAIOptions.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<AzureOpenAIOptions>() ?? new AzureOpenAIOptions();
        options.Invoke(configuredOptions);
        return services.AddAzureOpenAI(configuredOptions);
    }
}
