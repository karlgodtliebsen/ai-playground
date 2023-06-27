namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// HealthCheck Configurator
/// </summary>
public static class HealthCheckConfigurator
{
    /// <summary>
    /// Add HealthCheck
    /// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IHealthChecksBuilder to enable further expansion</returns>
    public static IHealthChecksBuilder AddHealthCheck(this IServiceCollection services)
    {
        IHealthChecksBuilder builder = services.AddHealthChecks();
        return builder;
    }

    /// <summary>
    /// Use HealthCheck in Anonymous mode
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder UseHealthCheck(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health").AllowAnonymous();
        return app;
    }
}