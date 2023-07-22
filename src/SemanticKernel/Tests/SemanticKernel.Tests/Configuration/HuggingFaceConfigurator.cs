using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SemanticKernel.Tests.Configuration;

public static class HuggingFaceConfigurator
{


    public static IServiceCollection AddHuggingFace(this IServiceCollection services, HuggingFaceOptions options)
    {
        services.AddSingleton<IOptions<HuggingFaceOptions>>(new OptionsWrapper<HuggingFaceOptions>(options));
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
        if (sectionName is null)
        {
            sectionName = HuggingFaceOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<HuggingFaceOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddHuggingFace(configuredOptions);
    }
}