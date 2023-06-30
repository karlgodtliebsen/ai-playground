using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// Extension for Api Version Configuration
/// </summary>
public static class ApiVersionConfigurator
{
    /// <summary>
    /// Add Api Version Rules. Defaults to  majorVersion = 1 and minorVersion = 0
    /// https://blog.christian-schou.dk/how-to-use-api-versioning-in-net-core-web-api/
    /// </summary>
    /// <param name="services"></param>
    /// <param name="majorVersion">default 1</param>
    /// <param name="minorVersion">default 0</param>
    /// <returns></returns>
    public static IServiceCollection AddApiVersionRules(this IServiceCollection services, int majorVersion = 1, int minorVersion = 0)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(majorVersion, minorVersion);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        });
        return services;
    }
}
