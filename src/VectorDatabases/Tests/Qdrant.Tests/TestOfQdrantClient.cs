using AI.VectorDatabaseQdrant.Configuration;
using AI.VectorDatabaseQdrant.VectorStorage;
using AI.VectorDatabaseQdrant.VectorStorage.Models;
using AI.VectorDatabaseQdrant.VectorStorage.Models.Payload;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Qdrant.Tests.Utils;


using Xunit.Abstractions;


namespace Qdrant.Tests;


public class TestOfQdrant
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly QdrantOptions options;


    public TestOfQdrant(ITestOutputHelper output)
    {
        this.output = output;
        this.factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddQdrant(configuration);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();
        options = factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
    }
    /*
     https://github.com/qdrant/qdrant/blob/master/QUICK_START.md
    docker run -p 6333:6333 qdrant/qdrant

    docker run -p 6333:6333 \
    -v $(pwd)/path/to/data:/qdrant/storage \
    -v $(pwd)/path/to/custom_config.yaml:/qdrant/config/production.yaml \
    qdrant/qdrant
    */

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
