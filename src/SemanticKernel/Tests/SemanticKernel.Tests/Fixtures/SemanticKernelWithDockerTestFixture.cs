using Embeddings.Qdrant.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

namespace SemanticKernel.Tests.Fixtures;

public sealed class SemanticKernelWithDockerTestFixture : SemanticKernelTestFixtureBase, IDisposable
{


    public TestContainerDockerLauncher Launcher { get; private set; }
    public Func<bool> Launch { get; set; }

    public SemanticKernelWithDockerTestFixture()
    {
        Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
        Launcher.Start();
    }

    public void Dispose()
    {
        Launcher.Stop();
    }
}
