using Destructurama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace OpenAI.Client.Configuration;

public static class ObservabilityConfiguration
{
    /// <summary>
    /// Configures/wires up the logging adjusted at web hosting
    /// Focus is on using Serilog and optionally adding telemetry logging
    /// The later requires a key
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddWebHostLogging(this WebApplicationBuilder builder,
        Action<LoggingOptions>? options = null)
    {
        var setOptions = new LoggingOptions();
        options?.Invoke(setOptions);
        builder.Logging.ClearProviders();
        builder.Host.AddHostLogging();
        if (setOptions.UseRequestResponseLogging)
        {
            builder.Services.AddRequestResponseLogging();
        }

        return builder;
    }

    /// <summary>
    /// AddHostLogging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IHostBuilder AddHostLogging(this IHostBuilder builder, Action<LoggingOptions>? options = null)
    {
        var setOptions = new LoggingOptions();
        options?.Invoke(setOptions);
        builder.UseSerilog((ctx, sp, lc) =>
        {
            lc.ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .Destructure.UsingAttributes();
        });
        return builder;
    }

    /// <summary>
    /// AddHostLogging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static HostApplicationBuilder AddHostLogging(this HostApplicationBuilder builder,
        Action<LoggingOptions>? options = null)
    {
        Log.Information("Application {name} is running in Environment {environment}",
            builder.Environment.ApplicationName, builder.Environment.EnvironmentName);

        var setOptions = new LoggingOptions();
        options?.Invoke(setOptions);
        builder.Logging.ClearProviders();
        var logger = Observability.CreateConfigurationBasedLogger(builder.Configuration);
        builder.Logging.AddSerilog(logger, false);
        builder.Services.AddSingleton<ILogger>(logger);
        return builder;
    }

    /// <summary>
    /// AddRequestResponseLogging
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRequestResponseLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(logging => { logging.LoggingFields = HttpLoggingFields.All; });
        return services;
    }

    /// <summary>
    /// UseLogging
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}