using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Domain;
using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;


namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample19Qdrant
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly IServiceProvider services;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIOptions openAIOptions;
    private readonly QdrantOptions qdrantOptions;

    public TestOfSemanticKernelExample19Qdrant(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.BuildFactoryWithLogging(output).WithDockerSupport();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.msLogger = services.GetRequiredService<ILogger<TestOfSemanticKernel>>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        this.qdrantOptions = services.GetRequiredService<IOptions<QdrantOptions>>().Value;
    }

    private const string collectionName = "SemanticKernel-19-test-collection";


    [Fact]
    public async Task UseQdrantMemoryCollection_Example19()
    {
        var url = this.qdrantOptions.Url;
        var completionModel = "text-davinci-003";
        var embeddingModel = "text-embedding-ada-002";
        const int openAiVectorSize = 1536;
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        var memoryStorage = await factory.Create(collectionName, openAiVectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = Kernel.Builder
            .WithLogger(msLogger)
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
