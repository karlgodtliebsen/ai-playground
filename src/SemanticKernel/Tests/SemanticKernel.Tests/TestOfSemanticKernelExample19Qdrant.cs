using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Domain;
using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

//using Azure.Core;
//using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;


namespace SemanticKernel.Tests;

[Collection("SemanticKernel With Docker Collection")]
public class TestOfSemanticKernelExample19Qdrant
{
    private readonly ILogger logger;

    private readonly HostApplicationFactory hostApplicationFactory;

    private readonly OpenAIOptions openAIOptions;
    private readonly QdrantOptions qdrantOptions;
    private readonly SemanticKernelWithDockerTestFixture fixture;

    public TestOfSemanticKernelExample19Qdrant(SemanticKernelWithDockerTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;

        this.hostApplicationFactory = fixture.Factory;
        this.openAIOptions = fixture.OpenAIOptions;
        this.qdrantOptions = fixture.QdrantOptions;
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
            .WithLogger(fixture.MsLogger)
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
