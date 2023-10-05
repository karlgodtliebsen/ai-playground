using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests;

[Collection("Docker Launch Collection")]
public class TestOfDockerLauncherWithAsyncLifeTime : IAsyncLifetime
{

    private readonly DockerLaunchTestFixture fixture;
    private readonly HostApplicationFactory factory;

    public TestOfDockerLauncherWithAsyncLifeTime(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {
        this.fixture = fixture;
        factory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithDockerSupport().Build();
    }

#pragma warning disable xUnit1013
    public async Task InitializeAsync()
#pragma warning restore xUnit1013
    {
        await this.fixture.Launcher?.StartAsync(CancellationToken.None)!;
    }

#pragma warning disable xUnit1013
    public async Task DisposeAsync()
#pragma warning restore xUnit1013
    {
        await this.fixture.Launcher?.StopAsync(CancellationToken.None)!;
    }

    [Fact]
    public void RunDockerLaunchTest()
    {
        factory.Should().NotBeNull();
        fixture.Should().NotBeNull();
    }
}
