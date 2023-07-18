using AI.Library.Configuration;
using AI.Library.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support;

public class HostApplicationFactory
{
    public IDateTimeProvider DateTimeProvider { get; private set; } = null!;
    public Serilog.ILogger Logger() => Services.GetRequiredService<ILogger>();
    public Microsoft.Extensions.Logging.ILogger MsLogger() => Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<object>>();

    public IServiceProvider Services { get; private set; } = null!;

    public static HostApplicationFactory Build(
                                                Func<string>? environment = null,
                                                Action<IServiceCollection, IConfigurationRoot>? serviceContext = null,
                                                Func<DateTimeOffset>? fixedDateTime = null,
                                                Func<ITestOutputHelper>? output = null)
    {
        var instance = new HostApplicationFactory();
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
        var env = environment?.Invoke();
        if (env is not null)
        {
            configurationBuilder.AddJsonFile($"appsettings.{env}.json", optional: true);
        }
        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddUserSecrets<HostApplicationFactory>();

        IConfigurationRoot configuration = configurationBuilder.Build();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceContext?.Invoke(serviceCollection, configuration);

        if (output is not null)
        {
            var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(configuration);
            cfg = cfg
                .MinimumLevel.Debug()
                .WriteTo.Sink(new LazyTestOutputHelperSink(output));
            Log.Logger = cfg.CreateLogger();
        }
        else
        {
            Log.Logger = Observability.CreateAppSettingsBasedLogger(configuration);
        }

        serviceCollection.AddSingleton(Log.Logger);
        serviceCollection.AddSingleton<ILoggerFactory>(new XUnitTestLoggerFactory(Log.Logger));

        instance.Services = serviceCollection.BuildServiceProvider();
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

        return instance;
    }
}
