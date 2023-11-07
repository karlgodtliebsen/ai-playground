using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SemanticKernel.Domain.Configuration;

public static class HuggingFaceConfigurator
{


    public static IServiceCollection AddHuggingFace(this IServiceCollection services, HuggingFaceOptions options)
    {
        services.AddSingleton<IOptions<HuggingFaceOptions>>(Options.Create(options));
        return services;
    }

    public static IServiceCollection AddHuggingFace(this IServiceCollection services, Action<HuggingFaceOptions>? options = null)
    {
        var configuredOptions = new HuggingFaceOptions();
        options?.Invoke(configuredOptions);
        return services.AddHuggingFace(configuredOptions);
    }

    public static IServiceCollection AddHuggingFace(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= HuggingFaceOptions.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<HuggingFaceOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddHuggingFace(configuredOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (HuggingFace) or the provided section name
    /// and add programmatically customizable configuration for the Inference Options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddHuggingFace(this IServiceCollection services, IConfiguration configuration, Action<HuggingFaceOptions> options, string? sectionName = null)
    {
        sectionName ??= HuggingFaceOptions.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<HuggingFaceOptions>() ?? new HuggingFaceOptions();
        options.Invoke(configuredOptions);
        return services.AddHuggingFace(configuredOptions);
    }

}
