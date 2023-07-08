using AI.CaaP.Configuration;
using AI.CaaP.Repository.Configuration;
using AI.CaaP.WebAPI.Controllers;

using Microsoft.Extensions.Options;

namespace AI.CaaP.WebAPI.Configuration;

/// <summary>
/// WebApi Configuration 
/// </summary>
public static class WebApiConfigurator
{
    /// <summary>
    /// WebAPI options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, IConfiguration configuration, WebApiOptions options)
    {
        services
            .VerifyAndAddOptions(options)
            .AddCaaP(configuration)
            .AddUtilities()
            .AddRepository()
            .AddDatabaseContext(configuration)
            ;
        return services;
    }

    /// <summary>
    /// Add configuration from appsettings.json for the WebAPI parts (ie the not llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services,
        IConfiguration configuration,
        Action<WebApiOptions>? options = null)
    {
        var configuredOptions = new WebApiOptions();
        options?.Invoke(configuredOptions);
        return services.AddWebApiConfiguration(configuration, configuredOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the WebAPI parts (ie the not llama model parts)
    /// If validation is not required, then just bind the options directly
    /// IConfigurationSection section = configuration.GetSection(sectionName);
    /// var section = section.GetSection(sectionName);
    ///services.AddOptions&lt;WebApiOptions&gt;().Bind(section);
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="webApiOptionsSectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, IConfiguration configuration, string? webApiOptionsSectionName = null)
    {
        if (webApiOptionsSectionName is null)
        {
            webApiOptionsSectionName = WebApiOptions.SectionName;
        }
        var options = configuration.GetSection(webApiOptionsSectionName).Get<WebApiOptions>()!;
        ArgumentNullException.ThrowIfNull(options);
        return services.AddWebApiConfiguration(configuration, options);
    }

    private static IServiceCollection VerifyAndAddOptions(this IServiceCollection services, WebApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton<IOptions<WebApiOptions>>(new OptionsWrapper<WebApiOptions>(options));
        return services;
    }

    private static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddScoped<IUserIdProvider, UserIdProvider>()
            ;
        return services;
    }

}
