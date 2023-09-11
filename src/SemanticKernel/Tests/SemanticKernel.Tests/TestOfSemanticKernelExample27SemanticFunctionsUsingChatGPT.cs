﻿using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample27SemanticFunctionsUsingChatGPT : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly ILoggerFactory loggerFactory;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticKernelTestFixture fixture;

    const string Model = "gpt-3.5-turbo";

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    public TestOfSemanticKernelExample27SemanticFunctionsUsingChatGPT(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBase>(output).Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
    }


    [Fact]
    public async Task UseSemanticFunction_Example27()
    {
        logger.Information("======== Using Chat GPT model for text completion ========");

        IKernel kernel = new KernelBuilder()
            .WithLoggerFactory(loggerFactory)
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
