using AI.Test.Support;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample20
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfSemanticKernelExample20(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }

    [Fact(Skip = "get a huggingface key")]
    public async Task UseSemanticFunction_Example27()
    {
        var model = "gpt-3.5-turbo";

        //logger.Information("======== HuggingFace text completion AI ========");
        //IKernel kernel = new KernelBuilder()
        //    .WithLogger(fixture.MsLogger)
        //    .WithHuggingFaceTextCompletionService(
        //        //model: TestConfiguration.HuggingFace.ApiKey,
        //        //apiKey: TestConfiguration.HuggingFace.ApiKey
        //        )
        //    .Build();

        //const string FunctionDefinition = "Question: {{$input}}; Answer:";

        //var questionAnswerFunction = kernel.CreateSemanticFunction(FunctionDefinition);

        //var result = await questionAnswerFunction.InvokeAsync("What is New York?");

        //logger.Information(result.Result);

        //foreach (var modelResult in result.ModelResults)
        //{
        //    logger.Information(modelResult.GetHuggingFaceResult().AsJson());
        //}
    }
}
