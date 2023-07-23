using AI.Test.Support;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample12SequentialPlanner
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfSemanticKernelExample12SequentialPlanner(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Setup(output);
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }

    [Fact]
    public async Task UseStepwisePlanner_Example51()
    {
        logger.Information("======== Sequential Planner - Create and Execute Poetry Plan ========");


    }
}
