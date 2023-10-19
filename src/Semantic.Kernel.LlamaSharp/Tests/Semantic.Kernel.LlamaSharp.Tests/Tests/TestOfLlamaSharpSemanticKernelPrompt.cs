using System.Security.Cryptography;

using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using LLama;
using LLama.Common;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.SemanticKernel.ChatCompletion;
using LLamaSharp.SemanticKernel.TextCompletion;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("LlamaSharp SemanticKernel Collection")]
public class TestOfLlamaSharpSemanticKernelPrompt : IAsyncLifetime
{
    private readonly ILogger logger;

    private readonly ILoggerFactory loggerFactory;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly LlamaSharpSemanticKernelTestFixture fixture;
    private readonly LLamaModelOptions llamaModelOptions;
    private readonly InferenceOptions inferenceOptions;

    private const string CollectionName = "LlamaSharp-SemanticKernel-test-collection";

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }
    public TestOfLlamaSharpSemanticKernelPrompt(LlamaSharpSemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture
            .WithOutputLogSupport<TestFixtureBaseWithDocker>(output)
            //.WithQdrantSupport()
            .Build();
        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.inferenceOptions = services.GetRequiredService<IOptions<InferenceOptions>>().Value;
        this.llamaModelOptions = services.GetRequiredService<IOptions<LLamaModelOptions>>().Value;
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
    }
    /*
     llamasharp semantic kernel documentation
       ## ITextCompletion
      ```csharp
      using var model = LLamaWeights.LoadFromFile(parameters);
      // LLamaSharpTextCompletion can accept ILLamaExecutor. 
      var ex = new StatelessExecutor(model, parameters);
      var builder = new KernelBuilder();
      builder.WithAIService<ITextCompletion>("local-llama", new LLamaSharpTextCompletion(ex), true);
      ```
    */
    [Fact]
    public async Task ExecuteTest()
    {
        logger.Information("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example17_ChatGPT.cs");

        var modelPath = this.llamaModelOptions.ModelPath;
        var seed = 1337;

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
            // .WithMemoryStorage(new VolatileMemoryStore())
            .WithAIService<ITextCompletion>("local-llama", new LLamaSharpTextCompletion(ex), true);

        var kernel = builder.Build();

        var prompt = @"{{$input}}

One line TLDR with the fewest words.";

        ChatRequestSettings settings = new() { MaxTokens = 100 };
        var summarize = kernel.CreateSemanticFunction(prompt, requestSettings: settings);

        var text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        var text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";

        var result1 = await kernel.RunAsync(text1, summarize);
        logger.Information(result1.ToString());

        var result2 = await kernel.RunAsync(text2, summarize);
        logger.Information(result2.ToString());
    }
}
