using AI.Library.Utils;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.HuggingFace.TextCompletion;

using SemanticKernel.Tests.Configuration;
using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample20HuggingFace
{
    private readonly ILogger logger;
    private readonly ILoggerFactory loggerFactory;


    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly HuggingFaceOptions huggingFaceOptions;

    //"https://api-inference.huggingface.co/models
    const string endPoint = "https://api-inference.huggingface.co/models";
    const string Model = "gpt2-large";

    public TestOfSemanticKernelExample20HuggingFace(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture
            .WithOutputLogSupport(output)
            .WithDockerSupport()
            .Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
        huggingFaceOptions = services.GetRequiredService<IOptions<HuggingFaceOptions>>().Value;
    }


    //gpt2
    //gpt2-large

    //Not Ok
    //meta-llama/Llama-2-7b
    //vicuna/ggml-vicuna-13b-1.1
    //eachadea/ggml-vicuna-7b-1.1


    [Fact(Skip = "This call does not return a answer af upgrade.")]
    public async Task UseSemanticFunction_Example27()
    {
        logger.Information("======== HuggingFace text completion AI ========");
        IKernel kernel = new KernelBuilder()
            .WithLoggerFactory(loggerFactory)
            .WithHuggingFaceTextCompletionService(
                model: Model,
                apiKey: huggingFaceOptions.ApiKey
            )
            .Build();

        //const string functionDefinition = "Question: {{$input}}; Answer:";
        //var func = kernel.CreateSemanticFunction(functionDefinition);
        //var result = await func.InvokeAsync("What is New York?");

        var func = kernel.CreateSemanticFunction("List the two planets closest to '{{$input}}', excluding moons, using bullet points.");
        var result = await func.InvokeAsync("Jupiter");


        logger.Information(result.Result);
        result.ErrorOccurred.Should().BeFalse();

        foreach (var modelResult in result.ModelResults)
        {
            var answer = modelResult.GetHuggingFaceResult().ToJson();
            //  answer.Should().Contain("Answer: New York is");
            logger.Information(answer);
        }
    }

    //Semantic kernel: HuggingFaceTextCompletionTests
    [Fact]
    public async Task HuggingFaceLocalAndRemoteTextCompletionAsync()
    {
        // Arrange
        const string Input = "This is test";

        var huggingFaceLocal = new HuggingFaceTextCompletion(Model, endpoint: endPoint);
        var huggingFaceRemote = new HuggingFaceTextCompletion(Model, apiKey: huggingFaceOptions.ApiKey);

        // Act
        var localResponse = await huggingFaceLocal.CompleteAsync(Input, new CompleteRequestSettings());
        var remoteResponse = await huggingFaceRemote.CompleteAsync(Input, new CompleteRequestSettings());

        // Assert
        localResponse.Should().NotBeNullOrEmpty();
        remoteResponse.Should().NotBeNullOrEmpty();
        logger.Information(remoteResponse);
        logger.Information(localResponse);

        Assert.StartsWith(Input, localResponse, StringComparison.Ordinal);
        Assert.StartsWith(Input, remoteResponse, StringComparison.Ordinal);
    }

    //Semantic kernel: HuggingFaceTextCompletionTests
    [Fact]
    public async Task RemoteHuggingFaceTextCompletionWithCustomHttpClientAsync()
    {
        // Arrange
        const string Input = "This is test";

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(endPoint);

        var huggingFaceRemote = new HuggingFaceTextCompletion(Model, apiKey: huggingFaceOptions.ApiKey, httpClient: httpClient);

        // Act
        var remoteResponse = await huggingFaceRemote.CompleteAsync(Input, new CompleteRequestSettings());

        // Assert
        remoteResponse.Should().NotBeNullOrEmpty();
        logger.Information(remoteResponse);
        Assert.StartsWith(Input, remoteResponse, StringComparison.Ordinal);
    }


}
