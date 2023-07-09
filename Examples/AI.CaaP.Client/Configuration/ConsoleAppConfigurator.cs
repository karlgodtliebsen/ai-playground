using AI.CaaP.Configuration;
using AI.CaaP.Repository.Configuration;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AI.CaaP.Client.Configuration;

/// <summary>
/// 
/// </summary>
public static class ConsoleAppConfigurator
{
    public static HostApplicationBuilder AddSecrets<T>(this HostApplicationBuilder builder) where T : class
    {
        builder.Configuration.AddUserSecrets<T>();
        return builder;
    }

    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddQdrant(configuration)
            .AddCaaP(configuration)
            .AddRepository()
            .AddDatabaseContext(configuration)
             ;
    }
}