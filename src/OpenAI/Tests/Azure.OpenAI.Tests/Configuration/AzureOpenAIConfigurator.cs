using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Azure.OpenAI.Tests.Configuration;

public static class AzureOpenAIConfigurator
{

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, AzureOpenAIConfiguration configuration)
    {
        services.AddSingleton<IOptions<AzureOpenAIConfiguration>>(Options.Create(configuration));
        return services;
    }

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, Action<AzureOpenAIConfiguration>? options = null)
    {
        var configuredOptions = new AzureOpenAIConfiguration();
        options?.Invoke(configuredOptions);
        return services.AddAzureOpenAI(configuredOptions);
    }

    public static IServiceCollection AddAzureOpenAI(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = AzureOpenAIConfiguration.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<AzureOpenAIConfiguration>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddAzureOpenAI(configuredOptions);
    }

}
