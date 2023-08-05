﻿using AI.Library.Utils;
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
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly QdrantOptions options;


    public TestOfQdrantAdministration(VectorDbTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Setup(output);
        this.hostApplicationFactory = fixture.Factory;
        this.options = fixture.Options;
        this.logger = fixture.Logger;
    }


    private const string CollectionName = "qdrant-test-collection";
    private const int VectorSize = 4;

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
        var result = await client.RemoveCollection(CollectionName, CancellationToken.None);
        result.Switch(
            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
           );
    }

    [Fact]
    public async Task VerifyAddCollectionToQdrant()
    {
        await CleanupCollections();
        var vectorParams = new VectorParams(VectorSize, Distance.DOT, true);
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var client = await qdrantFactory.Create(CollectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);
        var response = await client.CreateCollection(CollectionName, vectorParams, CancellationToken.None);
        response.Switch(
            _ => { },
            error => throw new QdrantException(error.Error)
        );

        var result = await client.GetCollections(CancellationToken.None);
        result.Switch(
            r =>
            {
                r.Collections.Should().NotBeNull();
                r.Collections.Any(c => c.Name == CollectionName).Should().BeTrue();
            },
            error => throw new QdrantException(error.Error)
        );
    }
    [Fact]
    public async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(CollectionName, CancellationToken.None);
        result.Switch(
            _ => logger.Information($"{CollectionName} deleted"),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyCreateCollectionInVectorDb()
    {
        await CleanupCollection();

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(VectorSize, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);

        var result = await client.CreateCollection(CollectionName, vectorParams, CancellationToken.None);
        result.Switch(
            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );

        var collection = await client.GetCollection(CollectionName, CancellationToken.None);

        collection.Switch(
            collectionInfo =>
            {
                collectionInfo.Status.Should().Be(CollectionStatus.GREEN);
                logger.Information(collectionInfo.AsJson());
            },
            error => throw new QdrantException(error.Error)
        );
    }

}
