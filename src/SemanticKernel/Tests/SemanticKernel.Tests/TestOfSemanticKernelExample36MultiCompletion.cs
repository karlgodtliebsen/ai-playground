using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextCompletion;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample36MultiCompletion
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly OpenAIOptions openAIOptions;

    public TestOfSemanticKernelExample36MultiCompletion(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.BuildFactoryWithLogging(output);
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.msLogger = services.GetRequiredService<ILogger<TestOfSemanticKernel>>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
    }

    [Fact]
    public async Task UseTextCompletion_Example36()
    {
        var model = "text-davinci-003";
        logger.Information("======== Open AI - Multiple Text Completion ========");

        ITextCompletion textCompletion = new OpenAITextCompletion(model, openAIOptions.ApiKey);
        var requestSettings = new CompleteRequestSettings()
        {
            MaxTokens = 200,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
            Temperature = 1,
            TopP = 0.5,
            ResultsPerPrompt = 2,
        };

        var prompt = "Write one paragraph why AI is awesome";

        foreach (ITextResult completionResult in await textCompletion.GetCompletionsAsync(prompt, requestSettings))
        {
            logger.Information(await completionResult.GetCompletionAsync());
            logger.Information("-------------");
        }
    }
}
