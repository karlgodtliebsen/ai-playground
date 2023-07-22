using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SemanticKernel.Tests.Configuration;

public static class AzureOpenAIConfigurator
{

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, AzureOpenAIOptions options)
    {
        services.AddSingleton<IOptions<AzureOpenAIOptions>>(new OptionsWrapper<AzureOpenAIOptions>(options));
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
        if (sectionName is null)
        {
            sectionName = AzureOpenAIOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<AzureOpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddAzureOpenAI(configuredOptions);
    }
}
