using AI.Library.Configuration;
using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

using NSubstitute;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

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
                        Action<IServiceCollection, IConfiguration>? serviceContext = null,
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
        IConfiguration buildConfiguration = configurationBuilder.Build();
        var serviceCollection = new ServiceCollection();
        serviceContext?.Invoke(serviceCollection, buildConfiguration);
        SetupLogging(serviceCollection, buildConfiguration, output);
        var instance = new HostApplicationFactory(serviceCollection.BuildServiceProvider())
        {
            DateTimeProvider = SetupTestDateTime(fixedDateTime)
        };
        return instance;
    }

    private static void SetupLogging(IServiceCollection serviceCollection, IConfiguration buildConfiguration, ITestOutputHelper? output = null)
    {
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
                var useScopes = logging.UsesScopes();
                logging.AddConsole();
                logging.AddDebug();
                serviceCollection.AddSingleton<ILoggerFactory, XUnitTestMsLoggerFactory>();
                serviceCollection.AddSingleton<ILoggerProvider>(new XUnitConsoleMsLoggerProvider(Console.Out, useScopes));
            });
        }
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

    internal HostApplicationFactory WithDockerSupport(out TestContainerDockerLauncher? launcher)
    {
        launcher = Services.GetRequiredService<TestContainerDockerLauncher>();
        return this;
    }

}

