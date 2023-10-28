using System.Diagnostics;
using System.Reflection;

using Destructurama;

using Microsoft.Extensions.Configuration;

using Serilog;

namespace AI.Library.Configuration;

/// <summary>
/// Configures the Logging used by this application - Serilog - web related logging and telemetry
/// </summary>
public static class Observability
{
    /// <summary>
    /// A default logger used before the application is wired up
    /// </summary>
    /// <param name="name"></param>
    /// <param name="anchor"></param>
    public static void UseBootstrapLogger(string name, Type? anchor = null)
    {
        if (anchor is null) anchor = typeof(Observability);
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Log.Logger = CreateBootstrapLogger();
        string? version = anchor.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Log.Information("Starting Application {name}. Version: {version}", name, version);
    }

    /// <summary>
    /// Creates a logger configuration based on the appSettings.json
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    public static LoggerConfiguration CreateLoggerConfigurationUsingAppSettings(IConfiguration configuration, AppLoggingOptions? options = null)
    {
        var cfg = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.Debug()
            .ConfigureOpenTelemetry(options)
            .Destructure.UsingAttributes();
        return cfg;
    }

    /// <summary>
    /// Creates a minimum configuration
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static LoggerConfiguration CreateMinimumConfiguration(AppLoggingOptions? options = null)
    {
        var cfg = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .ConfigureOpenTelemetry(options)
            .Destructure.UsingAttributes();
        return cfg;
    }

    private static LoggerConfiguration ConfigureOpenTelemetry(this LoggerConfiguration configuration, AppLoggingOptions? options = null)
    {
        if (options?.OpenTelemetry is null) return configuration;

        configuration = configuration.WriteTo.OpenTelemetry(
            opt =>
            {
                opt.Endpoint = options.OpenTelemetry.Endpoint!;
                opt.Protocol = options.OpenTelemetry.Protocol;
                if (options.OpenTelemetry.ResourceAttributes is not null)
                {
                    foreach (var kvp in options.OpenTelemetry.ResourceAttributes)
                    {
                        opt.ResourceAttributes.Add(kvp.Key, kvp.Value);
                    }
                }
                if (options.OpenTelemetry.Headers is not null)
                {
                    foreach (var kvp in options.OpenTelemetry.Headers)
                    {
                        opt.Headers.Add(kvp.Key, kvp.Value);
                    }
                }
                if (options.OpenTelemetry.IncludedData.HasValue)
                {
                    opt.IncludedData = options.OpenTelemetry.IncludedData.Value;
                }
            }
        );
        return configuration;
    }

    /// <summary>
    /// Creates a logger configuration based on the appSettings.json
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    public static ILogger CreateAppSettingsBasedLogger(this IConfiguration configuration, AppLoggingOptions? options = null)
    {
        var logger = CreateLoggerConfigurationUsingAppSettings(configuration, options).CreateLogger();
        return logger;
    }

    /// <summary>
    /// Creates a logger configuration based on minimal bootstrap Configuration
    /// </summary>
    public static ILogger CreateBootstrapLogger(AppLoggingOptions? options = null)
    {
        var logger = CreateMinimumConfiguration(options).CreateBootstrapLogger();
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
