using Kernel.Memory.NewsFeed.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Kernel.Memory.NewsFeed.Producer.Host.Configuration;

public static class HostingConfigurator
{
    /// <summary>
    /// AddOpenAIConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddHosting(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Setting up using Applications Configuration");
        services
            .AddNewsFeedDomain(configuration)
            .AddKafkaProducerHosts(configuration)
            ;
        Log.Logger.Information("Completed Adding Applications Configuration");
        return services;
    }

    public static HostApplicationBuilder AddSecrets<T>(this HostApplicationBuilder builder) where T : class
    {
        builder.Configuration.AddUserSecrets<T>();
        return builder;
    }
}
