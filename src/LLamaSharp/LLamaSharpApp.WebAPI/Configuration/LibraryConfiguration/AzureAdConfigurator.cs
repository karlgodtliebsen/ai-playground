using System.IdentityModel.Tokens.Jwt;

using Microsoft.Identity.Web;

namespace LLamaSharpApp.WebAPI.Configuration.LibraryConfiguration;

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
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
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
