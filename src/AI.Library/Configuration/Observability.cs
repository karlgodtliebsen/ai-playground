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
    public static void StartLogging(string name, Type? anchor = null)
    {
        if (anchor is null) anchor = typeof(Observability);
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Log.Logger = CreateBootstrapLogger();
        string? version = anchor.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Log.Information("Starting Application {name}. Version: {version}", name, version);
    }

    /// <summary>
    /// Creates a logger configuration based on the appsettings.json
    /// </summary>
    /// <param name="name"></param>
    /// <param name="anchor"></param>
    public static LoggerConfiguration CreateLoggerConfigurationUsingAppSettings(IConfiguration configuration)
    {
        var cfg = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //  .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration)
            .Destructure.UsingAttributes();
        return cfg;
    }
    /// <summary>
    /// Creates a minimum configuration
    /// </summary>
    /// <returns></returns>
    public static LoggerConfiguration CreateMinimumConfiguration()
    {
        var cfg = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            //.WriteTo.Trace()
            .WriteTo.Debug()
            .Destructure.UsingAttributes();
        return cfg;
    }

    /// <summary>
    /// Creates a logger configuration based on the appsettings.json
    /// </summary>
    /// <param name="configuration"></param>
    public static ILogger CreateAppSettingsBasedLogger(IConfiguration configuration)
    {
        var logger = CreateLoggerConfigurationUsingAppSettings(configuration).CreateLogger();
        return logger;
    }


    /// <summary>
    /// Creates a logger configuration based on minimal bootstrap Configuration
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
