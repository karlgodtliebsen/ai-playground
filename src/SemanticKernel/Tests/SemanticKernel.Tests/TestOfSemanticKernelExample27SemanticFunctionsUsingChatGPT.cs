using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample27SemanticFunctionsUsingChatGPT
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly OpenAIOptions openAIOptions;

    public TestOfSemanticKernelExample27SemanticFunctionsUsingChatGPT(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithOutputLogSupport(output).Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.msLogger = services.GetRequiredService<ILogger<TestOfSemanticKernel>>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
    }

    const string Model = "gpt-3.5-turbo";

    [Fact]
    public async Task UseSemanticFunction_Example27()
    {
        logger.Information("======== Using Chat GPT model for text completion ========");

        IKernel kernel = new KernelBuilder()
            .WithLogger(msLogger)
            .WithOpenAIChatCompletionService(Model, openAIOptions.ApiKey)
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
