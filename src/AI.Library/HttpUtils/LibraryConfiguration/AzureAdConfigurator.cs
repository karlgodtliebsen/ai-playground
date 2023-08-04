using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace AI.Library.HttpUtils.LibraryConfiguration;

/// <summary>
/// Azure AD Configurator
/// </summary>
public static class AzureAdConfigurator
{
    /// <summary>
    /// Configure Azure AD JWT Bearer Token
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdConfiguration(this IServiceCollection services, IConfiguration configuration, AzureAdOptions options, string? sectionName = default)
    {
        sectionName ??= AzureAdOptions.SectionName;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        //this part is a little over engineered. But it proves usage of the configuration pattern
        services.AddMicrosoftIdentityWebApiAuthentication(configuration, jwtBearerScheme: options.JwtBearerScheme, configSectionName: sectionName,
            subscribeToJwtBearerMiddlewareDiagnosticsEvents: options.SubscribeToJwtBearerMiddlewareDiagnosticsEvents);
        return services;
    }

    /// <summary>
    /// Configure Azure AD  Bearer Token
    /// Add Default configuration for programmatic configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdConfiguration(this IServiceCollection services, IConfiguration configuration, Action<AzureAdOptions>? options = default)
    {
        var configOptions = new AzureAdOptions();
        options?.Invoke(configOptions);
        return AddAzureAdConfiguration(services, configuration, configOptions);
    }

    /// <summary>
    /// Configure Azure AD Bearer Token
    /// Add configuration from configuration using default sectio nname (OpenApi) or the provided section name
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = default)
    {
        sectionName ??= AzureAdOptions.SectionName;
        var options = configuration.GetSection(sectionName).Get<AzureAdOptions>()!;
        return services.AddAzureAdConfiguration(configuration, options);
    }

    /// <summary>
    /// Add configuration from configuration using default section name (OpenApi) or the provided section name
    /// and allows for overriding of the configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdConfiguration(this IServiceCollection services, IConfiguration configuration, Action<AzureAdOptions> options, string? sectionName = null)
    {
        sectionName ??= AzureAdOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<AzureAdOptions>() ?? new AzureAdOptions();
        options.Invoke(modelOptions);
        return services.AddAzureAdConfiguration(configuration, modelOptions);
    }
}
