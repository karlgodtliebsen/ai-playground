using Destructurama;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;

using System.Diagnostics;
using System.Reflection;

namespace OpenAI.Client.Configuration;

/// <summary>
/// Configures the Logging used by this application - Serilog - web related logging and telemetry
/// </summary>
public static class Observability
{
    /// <summary>
    /// 
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