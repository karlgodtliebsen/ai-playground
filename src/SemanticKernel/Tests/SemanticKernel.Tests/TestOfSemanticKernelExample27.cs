using AI.Test.Support;

using Microsoft.SemanticKernel;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample27
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfSemanticKernelExample27(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }

    [Fact]
    public async Task UseSemanticFunction_Example27()
    {
        var model = "gpt-3.5-turbo";
        logger.Information("======== Using Chat GPT model for text completion ========");

        IKernel kernel = new KernelBuilder()
            .WithLogger(fixture.MsLogger)
            .WithOpenAIChatCompletionService(model, fixture.OpenAIOptions.ApiKey)
            .Build();

        var func = kernel.CreateSemanticFunction("List the two planets closest to '{{$input}}', excluding moons, using bullet points.");

        var result = await func.InvokeAsync("Jupiter");
        logger.Information(result.Result);

        /*
        Output:
           - Saturn
           - Uranus
        */

    }
}
