using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinancialAgents.Tests.Configuration;

public static class GoogleSearchConfigurator
{
    public static IServiceCollection AddGoogleSearch(this IServiceCollection services, GoogleOptions options)
    {
        services.AddSingleton<IOptions<GoogleOptions>>(Options.Create(options));
        return services;
    }

    public static IServiceCollection AddGoogleSearch(this IServiceCollection services, Action<GoogleOptions>? options = null)
    {
        var configuredOptions = new GoogleOptions();
        options?.Invoke(configuredOptions);
        return services.AddGoogleSearch(configuredOptions);
    }

    public static IServiceCollection AddGoogleSearch(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = GoogleOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<GoogleOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddGoogleSearch(configuredOptions);
    }
}
