using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Extensions.Logging;

namespace AI.Library.Configuration;


/// <summary>
/// Configures/wires up the Serilog based logging aimed for web hosting
/// Both Serilog and its use of OpenTelemetry is wired up
/// </summary>
public static class ObservabilityConfigurator
{
    /// <summary>
    /// Configures/wires up the Serilog based logging aimed for web hosting
    /// Both Serilog and its use of OpenTelemetry is wired up
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static WebApplicationBuilder WithLogging(this WebApplicationBuilder builder, Action<AppLoggingOptions>? options = null, string? sectionName = null)
    {
        var configuredOptions = GetLoggingOptions(builder.Configuration, options, sectionName);
        builder.Logging.ClearProviders();
        var logger = builder.Configuration.CreateAppSettingsBasedLogger(configuredOptions);
        builder.Logging.AddSerilog(logger, false);
        builder.Services.AddSingleton<ILogger>(logger);
        builder.Services.AddRequestResponseLogging(configuredOptions);
        return builder;
    }

    /// <summary>
    /// Configures/wires up the Serilog based logging aimed for web hosting
    /// Both Serilog and its use of OpenTelemetry is wired up
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IHostBuilder WithLogging(this IHostBuilder builder, //, IConfiguration configuration, IConfigurationBuilder cfgBuilder,
                                            Action<AppLoggingOptions>? options = null, string? sectionName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices((ctx, services) =>
        {
            var configuredOptions = GetLoggingOptions(ctx.Configuration, options, sectionName);
            var logger = ctx.Configuration.CreateAppSettingsBasedLogger(configuredOptions);
            services.AddSingleton<ILogger>(logger);
            services.AddRequestResponseLogging(configuredOptions);
            services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(_ => new SerilogLoggerProvider(logger, false));
        });
        return builder;
    }

    /// <summary>
    /// Configures/wires up the Serilog based logging aimed for web hosting
    /// Both Serilog and its use of OpenTelemetry is wired up
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static HostApplicationBuilder WithLogging(this HostApplicationBuilder builder,
                                                    Action<AppLoggingOptions>? options = null,
                                                    string? sectionName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        Log.Information("Application {name} is running in Environment {environment}", builder.Environment.ApplicationName, builder.Environment.EnvironmentName);
        var configuredOptions = GetLoggingOptions(builder.Configuration, options, sectionName);
        builder.Logging.ClearProviders();
        var logger = builder.Configuration.CreateAppSettingsBasedLogger(configuredOptions);
        builder.Logging.AddSerilog(logger, false);
        builder.Services.AddSingleton<ILogger>(logger);
        return builder;
    }

    public static AppLoggingOptions GetLoggingOptions(IConfiguration configuration, Action<AppLoggingOptions>? options, string? sectionName)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        if (options is not null && sectionName is not null)
        {
            var opt = configuration.GetSection(sectionName).Get<AppLoggingOptions>();
            if (opt is not null)
            {
                options.Invoke(opt);
                return opt;
            }
        }

        if (options is null)
        {
            sectionName ??= AppLoggingOptions.SectionName;
            var opt = configuration.GetSection(sectionName).Get<AppLoggingOptions>();
            if (opt is not null)
            {
                return opt;
            }
        }

        var configuredOptions = new AppLoggingOptions();
        options?.Invoke(configuredOptions);
        return configuredOptions;
    }

    /// <summary>
    /// Add RequestResponseLogging is configured by LoggingOptions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddRequestResponseLogging(this IServiceCollection services, AppLoggingOptions? options = null)
    {
        if (options is null || !options.UseRequestResponseLogging) return services;
        services.AddHttpLogging(logging => { logging.LoggingFields = HttpLoggingFields.All; });
        return services;
    }
}
