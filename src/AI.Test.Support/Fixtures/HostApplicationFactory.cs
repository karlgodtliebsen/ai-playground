using AI.Library.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

//public sealed class HostApplicationFactory
//{
//    public IDateTimeProvider DateTimeProvider { get; private set; } = null!;
//    public ILogger Logger() => Services.GetRequiredService<ILogger>();

//    public Microsoft.Extensions.Logging.ILogger MsLogger() => Services.GetRequiredService<ILogger<object>>();
//    public ILogger<T> MsLogger<T>() => Services.GetRequiredService<ILogger<T>>();
//    private readonly IConfigurationRoot configuration;
//    public IServiceProvider Services { get; private set; }

//    public void ConfigureLogging(ITestOutputHelper output)
//    {
//        var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(configuration);
//        cfg = cfg.WriteTo.TestOutput(output);
//        Log.Logger = cfg.CreateLogger();
//    }


//    public HostApplicationFactory(IConfigurationRoot configuration, IServiceProvider services)
//    {
//        Services = services;
//        this.configuration = configuration;
//    }


//    public static HostApplicationFactory Build(
//                                                Func<string>? environment = null,
//                                                Action<IServiceCollection, IConfigurationRoot>? serviceContext = null,
//                                                Func<DateTimeOffset>? fixedDateTime = null
//                                                )
//    {
//        var configurationBuilder = new ConfigurationBuilder();
//        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
//        var env = environment?.Invoke();
//        if (env is not null)
//        {
//            configurationBuilder.AddJsonFile($"appsettings.{env}.json", optional: true);
//        }
//        configurationBuilder.AddEnvironmentVariables();
//        configurationBuilder.AddUserSecrets<HostApplicationFactory>();

//        var buildConfiguration = configurationBuilder.Build();
//        var serviceCollection = new ServiceCollection();
//        serviceContext?.Invoke(serviceCollection, buildConfiguration);
//        serviceCollection
//        .AddTransient<ILogger>((_) => Log.Logger)
//        .AddSingleton<ILoggerFactory>(new XUnitTestLoggerFactory(Log.Logger))
//        .AddLogging(logging =>
//        {
//            logging.AddConsole();
//            logging.AddDebug();
//        });

//        var instance = new HostApplicationFactory(buildConfiguration, serviceCollection.BuildServiceProvider());

//        SetupDateTimeProvider(fixedDateTime, instance);

//        return instance;
//    }

//    private static void SetupDateTimeProvider(Func<DateTimeOffset>? fixedDateTime, HostApplicationFactory instance)
//    {
//        var fixedDt = fixedDateTime?.Invoke();
//        if (fixedDt is not null)
//        {
//            var dateTime = fixedDt.Value;

//            var dpMock = Substitute.For<IDateTimeProvider>();
//            dpMock.Now.Returns(dateTime);
//            instance.DateTimeProvider = dpMock;
//        }
//        else
//        {
//            instance.DateTimeProvider = new DateTimeProvider();
//        }
//    }
//}

public sealed class HostApplicationFactory
{
    public IServiceProvider Services { get; private set; }
    public ISystemClock DateTimeProvider { get; private set; } = default!;

    public HostApplicationFactory(IServiceProvider services)
    {
        Services = services;
    }

    public static HostApplicationFactory Build(
                        ITestOutputHelper? output = null,
                        Func<string>? environment = null,
                        Action<IServiceCollection, IConfigurationRoot>? serviceContext = null,
                        Func<DateTimeOffset>? fixedDateTime = null
                       )
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
        var env = environment?.Invoke();
        if (env is not null)
        {
            configurationBuilder.AddJsonFile($"appsettings.{env}.json", optional: true);
        }
        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddUserSecrets<HostApplicationFactory>();
        var buildConfiguration = configurationBuilder.Build();
        var serviceCollection = new ServiceCollection();
        serviceContext?.Invoke(serviceCollection, buildConfiguration);
        if (output is not null)
        {
            var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(buildConfiguration);
            cfg = cfg.WriteTo.TestOutput(output);
            Log.Logger = cfg.CreateLogger();

            serviceCollection
                .AddSingleton<ILogger>(Log.Logger)
                ;

            serviceCollection.AddLogging(logging =>
            {
                var useScopes = logging.UsesScopes();
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                serviceCollection.AddSingleton<ILoggerFactory, XUnitTestMsLoggerFactory>();
                serviceCollection.AddSingleton<ILoggerProvider>(new XUnitTestMsLoggerProvider(output, useScopes));
            });
        }
        else
        {
            serviceCollection.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
                serviceCollection.AddSingleton<ILoggerFactory, XUnitTestMsLoggerFactory>();
                serviceCollection.AddSingleton<ILoggerProvider>(new XUnitConsoleMsLoggerProvider(Console.Out, false));
            });
        }
        var instance = new HostApplicationFactory(serviceCollection.BuildServiceProvider())
        {
            DateTimeProvider = SetupTestDateTime(fixedDateTime)
        };
        return instance;
    }

    private static ISystemClock SetupTestDateTime(Func<DateTimeOffset>? fixedDateTime)
    {
        var fixedDt = fixedDateTime?.Invoke();
        if (fixedDt is null) return new SystemClock();
        var dateTime = fixedDt.Value;
        var dpMock = Substitute.For<ISystemClock>();
        dpMock
            .UtcNow
            .Returns(dateTime);
        return dpMock;
    }
}
