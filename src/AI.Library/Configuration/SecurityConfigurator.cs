using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace AI.Library.Configuration;

public static class SecurityConfigurator
{
    /// <summary>
    /// Add Security settings (authentication)
    /// <a href="https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens">Login and use ASP.NET Core API with Azure AD Auth and user access tokens</a>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration/*, Action<SecurityOptions>? options = null*/)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddMicrosoftIdentityWebApiAuthentication(configuration);

        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });
        return services;
    }
}
