using AI.Library.Configuration;
using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

public sealed class HostApplicationFactory : IDisposable
{
    public IServiceProvider Services { get; private set; }
    public ISystemClock DateTimeProvider { get; private set; } = default!;
    private TestContainerDockerLauncher? launcher = default!;

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

    public HostApplicationFactory WithDockerSupport()
    {
        launcher = Services.GetRequiredService<TestContainerDockerLauncher>();
        launcher.Start();
        return this;
    }
    public void Dispose()
    {
        launcher?.Stop();
        launcher = default;
    }

}

