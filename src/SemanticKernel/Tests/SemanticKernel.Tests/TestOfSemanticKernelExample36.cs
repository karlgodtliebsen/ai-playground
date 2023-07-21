﻿using AI.Test.Support;

using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextCompletion;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample36
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfSemanticKernelExample36(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }

    [Fact]
    public async Task UseTextCompletion_Example36()
    {
        var model = "text-davinci-003";
        logger.Information("======== Open AI - Multiple Text Completion ========");

        ITextCompletion textCompletion = new OpenAITextCompletion(model, fixture.OpenAIOptions.ApiKey);
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
