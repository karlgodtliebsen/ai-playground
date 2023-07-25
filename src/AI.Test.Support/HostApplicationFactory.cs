using AI.Library.Configuration;
using AI.Library.Utils;
using AI.Test.Support.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support;

public sealed class HostApplicationFactory
{
    public IDateTimeProvider DateTimeProvider { get; private set; } = null!;
    public ILogger Logger() => Services.GetRequiredService<ILogger>();

    public Microsoft.Extensions.Logging.ILogger MsLogger() => Services.GetRequiredService<ILogger<object>>();
    public ILogger<T> MsLogger<T>() => Services.GetRequiredService<ILogger<T>>();

    public void ConfigureLogging(ITestOutputHelper output)
    {
        var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(configuration);
        cfg = cfg.WriteTo.TestOutput(output);
        Log.Logger = cfg.CreateLogger();
    }

    private readonly IConfigurationRoot configuration;

    public HostApplicationFactory(IConfigurationRoot configuration, IServiceProvider services)
    {
        Services = services;
        this.configuration = configuration;
    }

    public IServiceProvider Services { get; private set; }

    public static HostApplicationFactory Build(
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
        serviceCollection
        .AddTransient<ILogger>((_) => Log.Logger)
        .AddSingleton<ILoggerFactory>(new XUnitTestLoggerFactory(Log.Logger))
        .AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        var instance = new HostApplicationFactory(buildConfiguration, serviceCollection.BuildServiceProvider());

        SetupDateTimeProvider(fixedDateTime, instance);

        return instance;
    }

    private static void SetupDateTimeProvider(Func<DateTimeOffset>? fixedDateTime, HostApplicationFactory instance)
    {
        var fixedDt = fixedDateTime?.Invoke();
        if (fixedDt is not null)
        {
            var dateTime = fixedDt.Value;
            var dpMock = new Mock<IDateTimeProvider>();
            dpMock
                .Setup((x) => x.Now)
                .Returns(dateTime);
            dpMock
                .Setup((x) => x.Now)
                .Returns(dateTime);
            instance.DateTimeProvider = dpMock.Object;
        }
        else
        {
            instance.DateTimeProvider = new DateTimeProvider();
        }
    }
}
