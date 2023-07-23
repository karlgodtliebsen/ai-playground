using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace SemanticKernel.Tests.Fixtures;

public sealed class SemanticKernelWithDockerTestFixture : SemanticKernelTestFixtureBase, IDisposable
{
    public TestContainerDockerLauncher Launcher { get; private set; }
    public Func<bool> Launch { get; set; }

    public SemanticKernelWithDockerTestFixture()
    {
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
