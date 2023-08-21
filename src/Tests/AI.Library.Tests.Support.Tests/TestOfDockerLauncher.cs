using AI.Library.Tests.Support.Tests.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Xunit.Abstractions;


namespace AI.Library.Tests.Support.Tests;

[Collection("Docker Launch Collection")]
public class TestOfDockerLauncher
{
    private readonly ITestOutputHelper output;

    private readonly DockerLaunchTestFixture fixture;
    public TestOfDockerLauncher(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {
        this.output = output;
        this.fixture = fixture;
    }

    [Fact]
    public void RunDockerLaunchTest()
    {
        var factory = fixture.WithOutputLogSupport(output).WithDockerSupport().Build();
        factory.Should().NotBeNull();
        var services = factory.Services;
        services.Should().NotBeNull();
        var logger = services.GetRequiredService<ILogger>();
        logger.Should().NotBeNull();
        logger.Information("Hello World");
    }

}
