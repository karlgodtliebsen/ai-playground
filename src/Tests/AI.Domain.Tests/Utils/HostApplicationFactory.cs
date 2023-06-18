using AI.Domain.Configuration;
using AI.Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Xunit.Abstractions;

namespace AI.Domain.Tests.Utils;

public class HostApplicationFactory
{
    public IDateTimeProvider DateTimeProvider { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;

    public static HostApplicationFactory Build(
        Func<string>? environment = null,
        Action<IServiceCollection, IConfigurationRoot>? serviceContext = null,
        Func<DateTimeOffset>? fixedDateTime = null,
        Func<ITestOutputHelper>? output = null)
    {
        var instance = new HostApplicationFactory();
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: true);
        var env = environment?.Invoke();
        if (env is not null)
        {
            builder.AddJsonFile($"appsettings.{env}.json", optional: true);
        }
        builder.AddEnvironmentVariables();

        IConfigurationRoot configuration = builder.Build();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceContext?.Invoke(serviceCollection, configuration);

        var outputHelper = output?.Invoke();
        if (outputHelper is not null)
        {
            var cfg = ObservabilityConfigurator.CreateLoggerConfigurationUsingAppConfiguration(configuration);
            cfg = cfg
                .MinimumLevel.Debug()
                .WriteTo.Sink(new TestOutputHelperSink(outputHelper));
            Log.Logger = cfg.CreateLogger();
        }
        else
        {
            Log.Logger = ObservabilityConfigurator.CreateConfigurationBasedLogger(configuration);
        }

        serviceCollection.AddSingleton(Log.Logger);

        instance.Services = serviceCollection.BuildServiceProvider();
        var fixedDT = fixedDateTime?.Invoke();
        if (fixedDT is not null)
        {
            var dateTime = fixedDT.Value;
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
