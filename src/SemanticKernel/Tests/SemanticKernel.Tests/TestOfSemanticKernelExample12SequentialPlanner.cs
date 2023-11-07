using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using SemanticKernel.Tests.Fixtures;
using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample12SequentialPlanner : IAsyncLifetime
{
    private readonly ILogger logger;

    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly SemanticKernelTestFixture fixture;

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    public TestOfSemanticKernelExample12SequentialPlanner(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
        services = hostApplicationFactory.Services;
        logger = services.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task UseStepwisePlanner_Example51()
    {
        logger.Information("======== Sequential Planner - Create and Execute Poetry Plan ========");

    }
}
