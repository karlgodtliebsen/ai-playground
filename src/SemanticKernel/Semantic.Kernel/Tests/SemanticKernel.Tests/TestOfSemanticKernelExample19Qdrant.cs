using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabases.MemoryStore.QdrantFactory;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;


namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample19Qdrant : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly ILoggerFactory loggerFactory;

    private readonly IServiceProvider services;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticKernelTestFixture fixture;
    private const string collectionName = "SemanticKernel-19-test-collection";

    public TestOfSemanticKernelExample19Qdrant(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
    }

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    [Fact]
    public async Task UseQdrantMemoryCollection_Example19()
    {
        var qdrantOptions = services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        var url = qdrantOptions.Endpoint;
        var completionModel = "text-davinci-003";
        var embeddingModel = "text-embedding-ada-002";
        const int openAiVectorSize = 1536;
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactoryForSemanticKernel>();
        var memoryStorage = await factory.Create(collectionName, openAiVectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = Kernel.Builder
            .WithLoggerFactory(loggerFactory)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService(embeddingModel, openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();

        logger.Information("== Printing Collections in DB ==");
        var collections = memoryStorage.GetCollectionsAsync();
        await foreach (var collection in collections)
        {
            logger.Information(collection);
        }

        logger.Information("== Adding Memories ==");

        var key1 = await kernel.Memory.SaveInformationAsync(collectionName, id: "cat1", text: "british short hair");
        var key2 = await kernel.Memory.SaveInformationAsync(collectionName, id: "cat2", text: "orange tabby");
        var key3 = await kernel.Memory.SaveInformationAsync(collectionName, id: "cat3", text: "norwegian forest cat");

        logger.Information("== Printing Collections in DB ==");
        collections = memoryStorage.GetCollectionsAsync();
        await foreach (var collection in collections)
        {
            logger.Information(collection);
        }

        logger.Information("== Retrieving Memories Through the Kernel ==");
        MemoryQueryResult? lookup = await kernel.Memory.GetAsync(collectionName, "cat1");
        logger.Information(lookup is not null ? lookup.Metadata.Text : "ERROR: memory not found");
        lookup.Should().NotBeNull();
        lookup!.Metadata.Should().NotBeNull();
        lookup!.Metadata.Text.Should().NotBeNull();
        lookup!.Metadata.Text.Should().Be("british short hair");

        logger.Information("== Retrieving Memories Directly From the Store ==");
        var memory1 = await memoryStorage.GetWithPointIdAsync(collectionName, key1);
        var memory2 = await memoryStorage.GetWithPointIdAsync(collectionName, key2);
        var memory3 = await memoryStorage.GetWithPointIdAsync(collectionName, key3);
        logger.Information(memory1 != null ? memory1.Metadata.Text : "ERROR: memory not found");
        logger.Information(memory2 != null ? memory2.Metadata.Text : "ERROR: memory not found");
        logger.Information(memory3 != null ? memory3.Metadata.Text : "ERROR: memory not found");
        memory1.Should().NotBeNull();
        memory2.Should().NotBeNull();
        memory3.Should().NotBeNull();


        logger.Information("== Similarity Searching Memories: My favorite color is orange ==");
        var searchResults = kernel.Memory.SearchAsync(collectionName, "My favorite color is orange", limit: 3, minRelevanceScore: 0.8);

        await foreach (var item in searchResults)
        {
            logger.Information(item.Metadata.Text + " : " + item.Relevance);
        }
    }
}
