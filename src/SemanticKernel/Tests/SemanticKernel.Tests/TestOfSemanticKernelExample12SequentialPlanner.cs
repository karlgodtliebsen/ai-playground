using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample12SequentialPlanner
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;

    public TestOfSemanticKernelExample12SequentialPlanner(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithLogging(output).WithDockerSupport().Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.msLogger = services.GetRequiredService<ILogger<TestOfSemanticKernel>>();
    }

    [Fact]
    public async Task UseStepwisePlanner_Example51()
    {
        logger.Information("======== Sequential Planner - Create and Execute Poetry Plan ========");


    }
}
