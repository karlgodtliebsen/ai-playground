﻿using AI.Test.Support;

using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextCompletion;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample37
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;
    private static readonly object s_lockObject = new();

    public TestOfSemanticKernelExample37(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }

    [Fact]
    public async Task UseTextCompletionStreamAsync_Example37()
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
            ResultsPerPrompt = 3
        };

        var prompt = "Write one paragraph why AI is awesome";
        var consoleLinesPerResult = 12;

        var resultTasks = new List<Task>();
        int currentResult = 0;
        await foreach (var completionResult in textCompletion.GetStreamingCompletionsAsync(prompt, requestSettings))
        {
            resultTasks.Add(ProcessStreamAsyncEnumerableAsync(completionResult, currentResult++, consoleLinesPerResult));
        }

        logger.Information("--------------------------------");

        await Task.WhenAll(resultTasks.ToArray());

        /*
        
        int position = 0;
        await foreach (ITextCompletionStreamingResult completionResult in textCompletion.CompleteMultiStreamAsync(prompt, requestSettings))
        {
            string fullMessage = string.Empty;
            await foreach (string message in completionResult.GetCompletionStreamingAsync())
            {
                fullMessage += message;
                Console.SetCursorPosition(0, (position * consoleLinesPerResult));
                Console.Write(fullMessage);
            }
                    position++;
        }*/
        //Console.SetCursorPosition(0, requestSettings.ResultsPerPrompt * consoleLinesPerResult);
        logger.Information("-------------------");

    }

    private static async Task ProcessStreamAsyncEnumerableAsync(ITextStreamingResult result, int resultNumber, int linesPerResult)
    {
        var fullSentence = string.Empty;
        await foreach (var word in result.GetCompletionStreamingAsync())
        {
            fullSentence += word;

            lock (s_lockObject)
            {
                //Console.SetCursorPosition(0, (resultNumber * linesPerResult));
                //Console.Write(fullSentence);
            }
        }
    }

}
