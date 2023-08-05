using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAgents.Tests.Configuration;

public static class SearchConfigurator
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(GoogleOptions.SectionName);
        services.AddOptions<GoogleOptions>().Bind(section);
        section = configuration.GetSection(BingOptions.SectionName);
        services.AddOptions<BingOptions>().Bind(section);
        return services;
    }

}
