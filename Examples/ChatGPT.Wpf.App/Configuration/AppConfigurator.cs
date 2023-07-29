using ChatGPT.Wpf.App.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Client.Configuration;

namespace ChatGPT.Wpf.App.Configuration;

/// <summary>
/// 
/// </summary>
public static class AppConfigurator
{
    public static HostApplicationBuilder AddSecrets<T>(this HostApplicationBuilder builder) where T : class
    {
        builder.Configuration.AddUserSecrets<T>();
        return builder;
    }
    public static IHostBuilder AddSecrets<T>(this IHostBuilder builder) where T : class
    {
        builder.ConfigureAppConfiguration((_, cb) =>
        {
            cb.AddUserSecrets<T>();
        });
        return builder;
    }
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ViewState>();
        return services.AddOpenAIConfiguration(configuration);
    }

}
