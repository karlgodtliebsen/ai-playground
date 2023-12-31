﻿using System.Security.Cryptography;

using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabases.MemoryStore.QdrantFactory;

using LLama;
using LLama.Common;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;


namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernel : IAsyncLifetime
{
    private readonly ILogger logger;

    private readonly ILoggerFactory loggerFactory;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticKernelTestFixture fixture;

    private const string CollectionName = "SemanticKernel-test-collection";

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }
    public TestOfSemanticKernel(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
    }

    [Fact]
    public async Task Run()
    {
        //Console.WriteLine("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example17_ChatGPT.cs");
        //Console.Write("Please input your model path: ");
        //var modelPath = Console.ReadLine();
        var modelPath = "text-davinci-003";

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            Seed = RandomNumberGenerator.GetInt32(int.MaxValue),
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        var ex = new StatelessExecutor(model, parameters);

        var builder = new KernelBuilder();
        builder
            .WithLoggerFactory(loggerFactory)
            .WithAIService<ITextCompletion>("local-llama", new LLamaSharpTextCompletion(ex), true);

        //var completionModel = "text-davinci-003";

        //var builder = new KernelBuilder();
        //builder
        //    .WithLoggerFactory(loggerFactory)
        //    .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey);


        var kernel = builder.Build();

        var prompt = @"{{$input}}

One line TLDR with the fewest words.";

        ChatRequestSettings settings = new() { MaxTokens = 100 };
        var summarize = kernel.CreateSemanticFunction(prompt, requestSettings: settings);

        string text1 = """
                        1st Law of Thermodynamics - Energy cannot be created or destroyed.
                        2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
                        3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.
                       """;

        string text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";
        var result1 = await kernel.RunAsync(text1, summarize);
        logger.Information(result1.ToString());

        var result2 = await kernel.RunAsync(text2, summarize);
        logger.Information(result2.ToString());
    }

    [Fact]
    public async Task RunSimpleSummarizationSample()
    {
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md
        var completionModel = "text-davinci-003";

        var builder = new KernelBuilder();
        builder
            .WithLoggerFactory(loggerFactory)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey);

        var kernel = builder.Build();
        var prompt = @"{{$input}}

One line TLDR with the fewest words.";

        ISKFunction summarize = kernel.CreateSemanticFunction(prompt);

        string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        string text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";


        var context1 = kernel.CreateNewContext();
        context1.Variables.Set("limit", "5");
        //context1.Variables.Set("limit", "5");
        context1.Variables.Set("relevance", "0.3");

        var context2 = kernel.CreateNewContext();
        context2.Variables.Set("limit", "5");
        context2.Variables.Set("relevance", "0.3");


        var r1 = await summarize.InvokeAsync(context1, cancellationToken: CancellationToken.None);
        var r2 = await summarize.InvokeAsync(context2, cancellationToken: CancellationToken.None);

        logger.Information(r1.ToString());
        logger.Information(r2.ToString());
    }

    [Fact]
    public async Task RunChainedSample()
    {
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md
        var completionModel = "text-davinci-003";
        var builder = new KernelBuilder();
        builder
            .WithLoggerFactory(loggerFactory)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey);

        var kernel = builder.Build();
        string translationPrompt = @"{{$input}}

Translate the text to math.";

        string summarizePrompt = @"{{$input}}

Give me a TLDR with the fewest words.";

        var translator = kernel.CreateSemanticFunction(translationPrompt);
        var summarize = kernel.CreateSemanticFunction(summarizePrompt);

        string inputText = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        // Run two prompts in sequence (prompt chaining)
        var output = await kernel.RunAsync(inputText, translator, summarize);
        logger.Information(output.ToString());
    }


    [Fact]
    public async Task RunTextSampleUsingSemanticKernel()
    {
        //https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/KernelSyntaxExamples
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs

        var completionModel = "text-davinci-003";
        var embeddingModel = "text-embedding-ada-002";
        const int openAiVectorSize = 1536;
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactoryForSemanticKernel>();
        var memoryStorage = await factory.Create(CollectionName, openAiVectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = Kernel.Builder
            .WithLoggerFactory(loggerFactory)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService(embeddingModel, openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();
        string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        await kernel.Memory.SaveReferenceAsync(
            collection: CollectionName,
            //description: text1,
            text: text1,
            externalId: Guid.NewGuid().ToString(),
            externalSourceName: "Uber"
        );
        logger.Information("saved.");

    }

}
