﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Library.HttpUtils.LibraryConfiguration;

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
        var builder = services.AddHealthChecks();
        return builder;
    }

    /// <summary>
    /// Map HealthCheck. Adds endpoint at /health
    /// </summary>
    /// <param name="app"></param>
    /// <param name="defaultEndpoint">"/health"</param>
    /// <returns>IEndpointConventionBuilder</returns>
    public static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder app, string defaultEndpoint = "/health")
    {
        return app.MapHealthChecks(defaultEndpoint);
    }

    /// <summary>
    /// Use HealthCheck in Anonymous mode. Adds endpoint at /health
    /// </summary>
    /// <param name="app"></param>
    /// <returns>IEndpointConventionBuilder</returns>
    public static IEndpointConventionBuilder MapHealthCheckAnonymous(this IEndpointRouteBuilder app)
    {
        return app.MapHealthCheck().AllowAnonymous();
    }
}
