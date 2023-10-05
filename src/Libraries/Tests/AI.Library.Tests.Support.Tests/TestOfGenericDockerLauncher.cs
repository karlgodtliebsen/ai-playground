using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Xunit.Abstractions;


namespace AI.Library.Tests.Support.Tests;

[Collection("Docker Launch Collection")]
public class TestOfGenericDockerLauncher
{
    private readonly ITestOutputHelper output;
    private readonly DockerLaunchTestFixture fixture;
    private readonly HostApplicationFactory factory;

    public TestOfGenericDockerLauncher(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {
        this.fixture = fixture;
        this.output = output;
        factory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithDockerSupport().Build();
    }


    [Fact]
    public async Task RunDockerLaunchTest()
    {
        factory.Should().NotBeNull();
        this.fixture.Launcher.Should().NotBeNull();
        var services = factory.Services;
        services.Should().NotBeNull();
        var logger = services.GetRequiredService<ILogger>();
        logger.Should().NotBeNull();
        logger.Information("Hello World");

        await this.fixture.Launcher?.StartAsync(CancellationToken.None)!;
        try
        {
            output.WriteLine($"Running");
        }
        finally
        {
            await this.fixture.Launcher?.StopAsync(CancellationToken.None)!;
        }
    }
}
