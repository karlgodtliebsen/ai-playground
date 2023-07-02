using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// Azure AD Configurator
/// </summary>
public static class AzureAdConfigurator
{
    /// <summary>
    /// Add Security settings (authentication) using Jwt Bearer Token
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration, Action<AzureAdOptions>? options = null)
    {
        var configOptions = new AzureAdOptions();
        options?.Invoke(configOptions);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization(opt =>
        {
            //opt.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => false).Build();
            opt.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
        services.AddMicrosoftIdentityWebApiAuthentication(configuration, configSectionName: configOptions.SectionName);
        return services;
    }


    /// <summary>
    /// Configure Azure AD  Bearer Token
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAd(this IServiceCollection services, IConfiguration configuration, Action<AzureAdOptions>? options = null)
    {
        var configOptions = new AzureAdOptions();
        options?.Invoke(configOptions);

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        IdentityModelEventSource.ShowPII = true;
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // OpenIdConnectDefaults.AuthenticationScheme
            ;
        services
            .AddMicrosoftIdentityWebApiAuthentication(configuration)
           ;

        // .AddMicrosoftIdentityWebApp(options => configuration.Bind("AzureAd", options))
        //services.AddMicrosoftIdentityWebApiAuthentication(configuration, configSectionName: configOptions.SectionName);

        return services;
    }

    ///// <summary>
    ///// Add Security settings (authentication) using Azure Ad
    ///// </summary>
    ///// <param name="services"></param>
    ///// <param name="configuration"></param>
    ///// <param name="options"></param>
    ///// <returns></returns>
    //public static IServiceCollection AddAzureAd(this IServiceCollection services, IConfiguration configuration, Action<AzureAdOptions>? options = null)
    //{
    //    var configOptions = new AzureAdOptions();
    //    options?.Invoke(configOptions);
    //    services
    //        .AddAuthentication(options =>
    //        {

    //        })

    //        .AddMicrosoftIdentityWebApp(configuration, configSectionName: configOptions.SectionName)
    //        .EnableTokenAcquisitionToCallDownstreamApi()
    //        .AddInMemoryTokenCaches()
    //        ;

    //    services.AddAuthorization(opt =>
    //    {
    //        //opt.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => false).Build();
    //        opt.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    //    });
    //    return services;
    //}

    //services.AddAuthentication(AzureADDefaults.AuthenticationScheme).AddAzureAD(options => builder.Configuration.Bind("AzureAd", options));

    //    services.AddAuthentication(options =>
    //    {
    //        options.DefaultAuthenticateScheme = "YourAuthenticationScheme";
    //        options.DefaultChallengeScheme = "YourAuthenticationScheme";
    //    });


    //services.AddAuthorization(options =>
    //{
    //    // Configure your authorization policies
    //});


}
