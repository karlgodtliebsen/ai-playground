using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

namespace FinancialAgents.Tests.Fixtures;

public class FinancialAgentsTestFixture : TestFixtureBase
{

}

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class FinancialAgentsWithDockerTestFixture : FinancialAgentsTestFixture, IDisposable
{
    // ReSharper disable once MemberCanBePrivate.Global
    public TestContainerDockerLauncher? Launcher { get; private set; } = default;
    public Func<bool> Launch { get; set; }

    /// <summary>
    /// Post Build Setup of Logging and Launcher that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    //public override void Setup(ITestOutputHelper output)
    //{
    //    //base.Setup(output);
    //    //Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
    //    //Launcher.Start();
    //}

    public void Dispose()
    {
        Launcher?.Stop();
    }
}

