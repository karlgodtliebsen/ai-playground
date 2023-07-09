using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Qdrant.Tests.Fixtures;

using Xunit.Abstractions;


namespace Qdrant.Tests;


[Collection("VectorDb Collection")]
public class TestOfQdrant
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly QdrantOptions options;


    public TestOfQdrant(VectorDbTestFixture fixture, ITestOutputHelper output)
    {
        this.output = output;
        this.factory = fixture.Factory;
        this.options = fixture.Options;
        this.logger = fixture.Logger;
        LaunchDocker.Launch();
    }


    private const string collectionName = "test-collection";

    [Fact]
    public async Task CleanupCollections()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(
            _ => { },
            error => throw new QdrantException(error.Error)
            );
    }


    [Fact]
    public async Task VerifyInitialAccessToQdrant()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.GetCollections(CancellationToken.None);
        result.Switch(
            collections => collections.Should().NotBeNull(),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyAddCollectionToQdrant()
    {
        await CleanupCollections();

        var vectorParams = new VectorParams(4, Distance.DOT, true);

        var client = factory.Services.GetRequiredService<IVectorDb>();

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

}
