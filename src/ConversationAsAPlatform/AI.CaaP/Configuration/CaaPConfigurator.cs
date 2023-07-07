using AI.CaaP.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AI.CaaP.Configuration;

public static class CaaPConfigurator
{

    public static IServiceCollection AddCaaP(this IServiceCollection services, CaaPOptions options)
    {
        services.AddSingleton<IOptions<CaaPOptions>>(new OptionsWrapper<CaaPOptions>(options));
        services.AddScoped<IAgentService, AgentService>();
        return services;
    }

    public static IServiceCollection AddCaaP(this IServiceCollection services, Action<CaaPOptions>? options = null)
    {
        var configuredOptions = new CaaPOptions();
        options?.Invoke(configuredOptions);
        return services.AddCaaP(configuredOptions);
    }

    public static IServiceCollection AddCaaP(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = CaaPOptions.ConfigSectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<CaaPOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddCaaP(configuredOptions);
    }

}
