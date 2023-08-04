using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Domain;
using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel With Docker Collection")]
public class TestOfSemanticKernelExample14SemanticMemory
{
    private readonly ILogger logger;

    private readonly HostApplicationFactory hostApplicationFactory;

    private readonly OpenAIOptions openAIOptions;
    private readonly QdrantOptions qdrantOptions;
    private readonly SemanticKernelWithDockerTestFixture fixture;

    public TestOfSemanticKernelExample14SemanticMemory(SemanticKernelWithDockerTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;

        this.hostApplicationFactory = fixture.Factory;
        this.openAIOptions = fixture.OpenAIOptions;
        this.qdrantOptions = fixture.QdrantOptions;
    }

    private const string CollectionName = "SemanticKernel-14-test-collection";
    private const string EmbeddingModel = "text-embedding-ada-002";
    private const int VectorSize = 1536;


    [Fact]
    public async Task RunSemanticMemory()
    {
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        var memoryStorage = await factory.Create(CollectionName, VectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        /* You can build your own semantic memory combining an Embedding Generator
         * with a Memory storage that supports search by similarity (ie semantic search).
         *
         * In this example we use qdrant as a memory storage.
         *
         * You can replace VolatileMemoryStore with Qdrant (see QdrantMemoryStore connector)
         * or implement your connectors for Pinecone, Vespa, Postgres + pgvector, SQLite VSS, etc.
         */

        var kernelWithCustomDb = Kernel.Builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextEmbeddingGenerationService(EmbeddingModel, fixture.OpenAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();

        await RunExampleAsync(kernelWithCustomDb);
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
    }

    private async Task SearchMemoryAsync(IKernel kernel, string query)
    {
        logger.Information("\nQuery: " + query + "\n");

        var memories = kernel.Memory.SearchAsync(CollectionName, query, limit: 2, minRelevanceScore: 0.5);

        int i = 0;
        await foreach (MemoryQueryResult memory in memories)
        {
            logger.Information($"Result {++i}:");
            logger.Information("  URL:     : " + memory.Metadata.Id);
            logger.Information("  Title    : " + memory.Metadata.Description);
            logger.Information("  Relevance: " + memory.Relevance);
            logger.Information("");
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
            await kernel.Memory.SaveReferenceAsync(
                collection: CollectionName,
                externalSourceName: "GitHub",
                externalId: entry.Key,
                description: entry.Value,
                text: entry.Value);

            Console.Write($" #{++i} saved.");
        }

        logger.Information("\n----------------------");
    }

    private Dictionary<string, string> SampleData()
    {
        return new Dictionary<string, string>
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/00-getting-started.ipynb"]
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
