using AI.Test.Support;
using AI.Test.Support.DockerSupport;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;

namespace Qdrant.Tests.Fixtures;

public sealed class VectorDbTestFixture : TestFixtureBase, IDisposable
{

    public QdrantOptions Options { get; private set; }
    public TestContainerDockerLauncher Launcher { get; private set; }

    public VectorDbTestFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddQdrant(configuration);
                services.AddSingleton<TestContainerDockerLauncher>();
                var section = configuration.GetSection(DockerLaunchOptions.SectionName);
                services.AddOptions<DockerLaunchOptions>().Bind(section);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        Options = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
    }
    /// <summary>
    /// Post Build Setup of Logging and Launcher that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    public override void Setup(ITestOutputHelper output)
    {
        base.Setup(output);
        Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
        Launcher.Start();
    }

    public void Dispose()
    {
        Launcher.Stop();
    }
}
