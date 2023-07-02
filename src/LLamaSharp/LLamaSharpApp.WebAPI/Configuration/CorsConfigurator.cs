namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// Cors Configurator
/// </summary>
public static class CorsConfigurator
{
    /// <summary>
    /// Configure Cors
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration, Action<CorsOptions>? options = null)
    {
        var configOptions = new CorsOptions();
        options?.Invoke(configOptions);
        services.AddCors(opt =>
        {
            opt.AddPolicy(configOptions.Policy,
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .WithOrigins(configOptions.Origins)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
        return services;
    }

}
