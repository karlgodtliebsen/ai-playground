using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Library.HttpUtils.LibraryConfiguration;

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
    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration, CorsOptions options)
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

    public static IServiceCollection AddCors(this IServiceCollection services, string origins)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(origins,
                policy =>
                {
                    policy
                        .AllowCredentials()
                        .WithOrigins()
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
    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration, Action<CorsOptions>? options = null)
    {
        var configOptions = new CorsOptions();
        options?.Invoke(configOptions);
        return services.AddCors(configuration, configOptions);
    }


    /// <summary>
    /// Configure Cors
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= CorsOptions.SectionName;
        var options = configuration.GetSection(sectionName).Get<CorsOptions>()!;
        return services.AddCors(configuration, options);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdAddCorsConfigConfiguration(this IServiceCollection services, IConfiguration configuration, Action<CorsOptions> options, string? sectionName = null)
    {
        sectionName ??= CorsOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<CorsOptions>() ?? new CorsOptions();
        options.Invoke(modelOptions);
        return services.AddCors(configuration, modelOptions);
    }

}
