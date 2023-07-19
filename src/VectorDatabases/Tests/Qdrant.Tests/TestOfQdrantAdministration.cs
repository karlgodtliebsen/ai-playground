using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Qdrant.Tests.Fixtures;

using Xunit.Abstractions;


namespace Qdrant.Tests;

[Collection("VectorDb Collection")]
public class TestOfQdrantAdministration
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly QdrantOptions options;


    public TestOfQdrantAdministration(VectorDbTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.output = output;
        this.hostApplicationFactory = fixture.Factory;
        this.options = fixture.Options;
        this.logger = fixture.Logger;
    }


    private const string collectionName = "test-collection";
    private const int vectorSize = 4;

    [Fact]
    public async Task VerifyInitialAccessToQdrant()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.GetCollections(CancellationToken.None);
        result.Switch(
            collections => collections.Should().NotBeNull(),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task CleanupCollections()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(
            _ => { logger.Information("Succeeded"); },
            error => throw new QdrantException(error.Error)
            );
    }

    [Fact]
    public async Task VerifyAddCollectionToQdrant()
    {
        await CleanupCollections();

        var vectorParams = new VectorParams(vectorSize, Distance.DOT, true);
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var client = await qdrantFactory.Create(collectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);
        var response = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        response.Switch(
            _ => { },
            error => throw new QdrantException(error.Error)
        );

        var result = await client.GetCollections(CancellationToken.None);
        result.Switch(
            r =>
            {
                r.Collections.Should().NotBeNull();
                r.Collections.Any(c => c.Name == collectionName).Should().BeTrue();
            },
            error => throw new QdrantException(error.Error)
        );
    }
    [Fact]
    public async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ =>
            {
                logger.Information($"{collectionName} deleted");
            },
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyCreateCollectionInVectorDb()
    {
        await CleanupCollection();

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(vectorSize, Distance.DOT, true);
        var client = await qdrantFactory.Create(collectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);

        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );

        var collection = await client.GetCollection(collectionName, CancellationToken.None);

        collection.Switch(

            collectionInfo =>
            {
                collectionInfo.Status.Should().Be(CollectionStatus.GREEN);
                output.WriteLine(JsonSerializer.Serialize(collectionInfo));
            },
            error => throw new QdrantException(error.Error)
        );
    }

}
