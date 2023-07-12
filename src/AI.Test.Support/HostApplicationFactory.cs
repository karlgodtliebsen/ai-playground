using AI.Library.Configuration;
using AI.Library.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support;

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
        builder.AddUserSecrets<HostApplicationFactory>();

        IConfigurationRoot configuration = builder.Build();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceContext?.Invoke(serviceCollection, configuration);

        if (output is not null)
        {
            var cfg = Observability.CreateLoggerConfigurationUsingAppConfiguration(configuration);
            cfg = cfg
                .MinimumLevel.Debug()
                .WriteTo.Sink(new LazyTestOutputHelperSink(output));
            Log.Logger = cfg.CreateLogger();
        }
        else
        {
            Log.Logger = Observability.CreateConfigurationBasedLogger(configuration);
        }

        serviceCollection.AddSingleton(Log.Logger);

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
