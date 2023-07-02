namespace LLamaSharpApp.WebAPI.Configuration.LibraryConfiguration;

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
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration, CorsOptions options)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(options.Policy,
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .WithOrigins(options.Origins)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
        return services;
    }

    /// <summary>
    /// Configure Cors
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration, Action<CorsOptions>? options = null)
    {
        var configOptions = new CorsOptions();
        options?.Invoke(configOptions);
        return services.AddCorsConfig(configuration, configOptions);
    }


    /// <summary>
    /// Configure Cors
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = CorsOptions.DefaultSectionName;
        }
        var options = configuration.GetSection(sectionName).Get<CorsOptions>()!;
        return services.AddCorsConfig(configuration, options);
    }

}
