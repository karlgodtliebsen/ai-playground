using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;

using Embeddings.Qdrant.Tests.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;

namespace Qdrant.Tests.Fixtures;

public sealed class VectorDbTestFixture : IDisposable
{

    private readonly Func<ITestOutputHelper>? getOutput;
    public ITestOutputHelper Output { get; set; }

    public QdrantOptions Options { get; private set; }

    public Serilog.ILogger Logger { get; private set; }
    public Microsoft.Extensions.Logging.ILogger MsLogger { get; set; }

    public HostApplicationFactory Factory { get; private set; }


    public TestContainerDockerLauncher Launcher { get; private set; }

    public VectorDbTestFixture()
    {
        getOutput = () => Output!;
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddQdrant(configuration);
                services.AddSingleton<TestContainerDockerLauncher>();
                var section = configuration.GetSection(DockerLaunchOptions.SectionName);
                services.AddOptions<DockerLaunchOptions>().Bind(section);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: getOutput
        );
        Logger = Factory.Logger();
        MsLogger = Factory.MsLogger();
        Options = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
        Launcher.Start();
    }

    public void Dispose()
    {
        Launcher.Stop();
    }
}
