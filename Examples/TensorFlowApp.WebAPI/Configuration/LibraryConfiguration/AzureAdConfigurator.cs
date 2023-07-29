using System.IdentityModel.Tokens.Jwt;
using Microsoft.Identity.Web;

namespace TensorFlowApp.WebAPI.Configuration.LibraryConfiguration;

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
    /// <returns></returns>
    public static IServiceCollection AddAzureAdConfiguration(this IServiceCollection services, IConfiguration configuration, AzureAdOptions options)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        //this part is a little overengineered. But it proves usage of the configuration pattern
        services.AddMicrosoftIdentityWebApiAuthentication(configuration, jwtBearerScheme: options.JwtBearerScheme, configSectionName: options.SectionName,
            subscribeToJwtBearerMiddlewareDiagnosticsEvents: options.SubscribeToJwtBearerMiddlewareDiagnosticsEvents);
        return services;
    }

    /// <summary>
    /// Configure Azure AD  Bearer Token
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
    /// Configure Azure AD  Bearer Token
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = default)
    {
        if (sectionName is null)
        {
            sectionName = AzureAdOptions.DefaultSectionName;
        }
        var options = configuration.GetSection(sectionName).Get<AzureAdOptions>()!;
        return services.AddAzureAdConfiguration(configuration, options);
    }

}
