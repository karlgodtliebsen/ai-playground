using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabases.MemoryStore.SemanticKernelSupport;

using LLama;
using LLama.Common;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.SemanticKernel.TextEmbedding;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.Embeddings;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("LlamaSharp SemanticKernel Collection")]
public class TestOfLlamaSharpSemanticKernelMemory //: IAsyncLifetime
{
    private readonly ILogger logger;

    private readonly ILoggerFactory loggerFactory;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly LlamaSharpSemanticKernelTestFixture fixture;
    private readonly InferenceOptions inferenceOptions;
    private readonly LLamaModelOptions llamaModelOptions;
    private readonly QdrantOptions qdrantOptions;
    private readonly IQdrantFactory qdrantFactory;
    private readonly IQdrantSemanticKernelMemoryStoreFactory memoryStoreFactory;

    private const string MemoryCollectionName = "LlamaSharp-SemanticKernel-test-collection";


    //public Task InitializeAsync()
    //{
    //    return fixture.InitializeAsync();
    //}

    //public Task DisposeAsync()
    //{
    //    return fixture.DisposeAsync();
    //}
    public TestOfLlamaSharpSemanticKernelMemory(LlamaSharpSemanticKernelTestFixture fixture, ITestOutputHelper output)
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
        this.qdrantOptions = services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        this.qdrantFactory = services.GetRequiredService<IQdrantFactory>();
        this.memoryStoreFactory = services.GetRequiredService<IQdrantSemanticKernelMemoryStoreFactory>();
    }

    /*
    llmasharp semantic kernel documentation
    ## ITextEmbeddingGeneration
    ```csharp
    using var model = LLamaWeights.LoadFromFile(parameters);
    var embedding = new LLamaEmbedder(model, parameters);
    var kernelWithCustomDb = Kernel.Builderhttp://browsing-file-host/LLama.Examples/NewVersion/SemanticKernelMemory.cs
        .WithLoggerFactory(ConsoleLogger.LoggerFactory)
        .WithAIService<ITextEmbeddingGeneration>("local-llama-embed", new LLamaSharpEmbeddingGeneration(embedding), true)
        .WithMemoryStorage(new VolatileMemoryStore())
        .Build();
    ```
    */

    [Fact]
    public async Task ExecuteTest()
    {
        logger.Information("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs");

        //QdrantVectorDbClient client = new QdrantVectorDbClient(qdrantOptions.Endpoint, qdrantOptions.DefaultDimension, loggerFactory);
        //var memoryStore = new QdrantMemoryStore(client, loggerFactory);

        //var memoryStore = new VolatileMemoryStore();
        var memoryStore = await memoryStoreFactory.Create(MemoryCollectionName, qdrantOptions.DefaultDimension, distance: qdrantOptions.DefaultDistance, cancellationToken: CancellationToken.None);

        var modelPath = this.llamaModelOptions.ModelPath;
        var seed = 1337;

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            Seed = seed,
            EmbeddingMode = true
        };

        using var model = LLamaWeights.LoadFromFile(parameters);
        var embedding = new LLamaEmbedder(model, parameters);

        logger.Information("====================================================");
        logger.Information("======== Semantic Memory (qdrant)===================");
        logger.Information("====================================================");

        var kernelWithCustomDb = Kernel.Builder
            .WithLoggerFactory(loggerFactory)
            .WithAIService<ITextEmbeddingGeneration>("local-llama-embed", new LLamaSharpEmbeddingGeneration(embedding), true)
            .WithMemoryStorage(memoryStore)
            ;

        var kernel = kernelWithCustomDb
            .Build();

        await RunExampleAsync(kernel);
    }

    private async Task RunExampleAsync(IKernel kernel)
    {
        await StoreMemoryAsync(kernel);

        await SearchMemoryAsync(kernel, "How do I get started?");

        /*
        Output:

        Query: How do I get started?

        Result 1:
          URL:     : https://github.com/microsoft/semantic-kernel/blob/main/README.md
          Title    : README: Installation, getting started, and how to contribute

        Result 2:
          URL:     : https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet-jupyter-notebooks/00-getting-started.ipynb
          Title    : Jupyter notebook describing how to get started with the Semantic Kernel

        */

        await SearchMemoryAsync(kernel, "Can I build a chat with SK?");

        /*
        Output:

        Query: Can I build a chat with SK?

        Result 1:
          URL:     : https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT
          Title    : Sample demonstrating how to create a chat skill interfacing with ChatGPT

        Result 2:
          URL:     : https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md
          Title    : README: README associated with a sample chat summary react-based webapp

        */

        await SearchMemoryAsync(kernel, "Jupyter notebook");

        await SearchMemoryAsync(kernel, "README: README associated with a sample chat summary react-based webapp");

        await SearchMemoryAsync(kernel, "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function");
    }

    private async Task SearchMemoryAsync(IKernel kernel, string query)
    {
        logger.Information("\nQuery: " + query + "\n");

        var memories = kernel.Memory.SearchAsync(MemoryCollectionName, query, limit: 10, minRelevanceScore: 0.5);

        var i = 0;
        await foreach (var memory in memories)
        {
            logger.Information($"Result {++i}:");
            logger.Information("  URL:     : " + memory.Metadata.Id);
            logger.Information("  Title    : " + memory.Metadata.Description);
            logger.Information("  Relevance: " + memory.Relevance);
            logger.Information("--");
        }

        logger.Information("----------------------");
    }

    private async Task StoreMemoryAsync(IKernel kernel)
    {
        /* Store some data in the semantic memory.
         *
         * When using Azure Cognitive Search the data is automatically indexed on write.
         *
         * When using the combination of VolatileStore and Embedding generation, SK takes
         * care of creating and storing the index
         */

        logger.Information("\nAdding some GitHub file URLs and their descriptions to the semantic memory.");
        var githubFiles = SampleData();
        var i = 0;
        foreach (var entry in githubFiles)
        {
            var result = await kernel.Memory.SaveReferenceAsync(
                collection: MemoryCollectionName,
                externalSourceName: "GitHub",
                externalId: entry.Key,
                description: entry.Value,
                text: entry.Value);

            logger.Information($"#{++i} saved.");
            logger.Information(result);
        }

        logger.Information("\n----------------------");
    }

    private Dictionary<string, string> SampleData()
    {
        return new Dictionary<string, string>
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks//00-getting-started.ipynb"]
                = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT"]
                = "Sample demonstrating how to create a chat skill interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
                = "C# class that defines a volatile embedding store",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
                = "README: README associated with a sample chat summary react-based webapp",
        };
    }
}
