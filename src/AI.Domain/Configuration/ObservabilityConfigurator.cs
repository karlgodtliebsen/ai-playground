using Destructurama;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using System.Diagnostics;
using System.Reflection;

namespace AI.Domain.Configuration;

/// <summary>
/// Configures the Logging used by this application - Serilog - web related logging and telemetry
/// </summary>
public static class ObservabilityConfigurator
{
    /// <summary>
    /// Configures/wires up the logging adjusted at web hosting
    /// Focus is on using Serilog and optionally adding telemetry logging
    /// The later requires a key
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddWebHostLogging(this WebApplicationBuilder builder, Action<LoggingOptions>? options = null)
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
    public static HostApplicationBuilder AddHostLogging(this HostApplicationBuilder builder, Action<LoggingOptions>? options = null)
    {
        Log.Information("Application {name} is running in Environment {environment}", builder.Environment.ApplicationName, builder.Environment.EnvironmentName);

        var setOptions = new LoggingOptions();
        options?.Invoke(setOptions);
        builder.Logging.ClearProviders();
        var logger = CreateConfigurationBasedLogger(builder.Configuration);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="anchor"></param>
    public static void StartLogging(string name, Type? anchor = null)
    {
        if (anchor is null) anchor = typeof(ObservabilityConfigurator);
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Log.Logger = CreateBootstrapLogger();
        string? version = anchor.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Log.Information("Starting Application {name}. Version: {version}", name, version);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="anchor"></param>
    public static LoggerConfiguration CreateLoggerConfigurationUsingAppConfiguration(IConfiguration configuration)
    {
        var cfg = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration)
            .Destructure.UsingAttributes();
        return cfg;
    }
    public static LoggerConfiguration CreateMinimumConfiguration()
    {
        var cfg = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Trace()
            .WriteTo.Debug()
            .Destructure.UsingAttributes();
        return cfg;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="anchor"></param>
    public static ILogger CreateConfigurationBasedLogger(IConfiguration configuration)
    {
        var logger = CreateLoggerConfigurationUsingAppConfiguration(configuration).CreateLogger();
        return logger;
    }


    /// <summary>
    /// 
    /// </summary>
    public static ILogger CreateBootstrapLogger()
    {
        var logger = CreateMinimumConfiguration().CreateBootstrapLogger();
        return logger;
    }


    /// <summary>
    /// LogFinalizedConfiguration
    /// </summary>
    /// <param name="name"></param>
    public static void LogFinalizedConfiguration(string name)
    {
        Log.Information("Application {name} is Wired Up and proceeding with final startup...", name);
    }

    /// <summary>
    /// Logs a Stop message and flushes the Logger
    /// </summary>
    /// <param name="name"></param>
    public static void StopLogging(string name)
    {
        Log.Information("Stopping Application {name}", name);
        Log.CloseAndFlush();
    }
}