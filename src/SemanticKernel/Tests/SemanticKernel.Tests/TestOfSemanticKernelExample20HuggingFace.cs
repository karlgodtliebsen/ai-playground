﻿using AI.Library.Utils;
using AI.Test.Support;

using FluentAssertions;

using Microsoft.SemanticKernel;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample20HuggingFace
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfSemanticKernelExample20HuggingFace(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }

    //"https://api-inference.huggingface.co/models

    const string Model = "gpt2-large";
    //Ok    
    //gpt2
    //gpt2-large

    //Not Ok
    //meta-llama/Llama-2-7b
    //vicuna/ggml-vicuna-13b-1.1
    //eachadea/ggml-vicuna-7b-1.1


    [Fact]
    public async Task UseSemanticFunction_Example27()
    {
        logger.Information("======== HuggingFace text completion AI ========");
        IKernel kernel = new KernelBuilder()
            .WithLogger(fixture.MsLogger)
            .WithHuggingFaceTextCompletionService(
                model: Model,
                apiKey: fixture.HuggingFaceOptions.ApiKey
            )
            .Build();

        const string functionDefinition = "Question: {{$input}}; Answer:";

        var questionAnswerFunction = kernel.CreateSemanticFunction(functionDefinition);

        var result = await questionAnswerFunction.InvokeAsync("What is New York?");

        logger.Information(result.Result);
        result.ErrorOccurred.Should().BeFalse();

        foreach (var modelResult in result.ModelResults)
        {
            var answer = modelResult.GetHuggingFaceResult().AsJson();
            answer.Should().Contain("Answer: New York is");
            logger.Information(answer);
        }
    }
}
