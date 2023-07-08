using AI.VectorDatabaseQdrant.Configuration;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Qdrant.Tests.Utils;

using QdrantCSharp;
using QdrantCSharp.Models;

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
        var client = factory.Services.GetRequiredService<QdrantHttpClient>();
        var result = await client.DeleteCollection(collectionName);
        result.Status.Should().Be("ok");
    }


    [Fact]
    public async Task VerifyInitialAccessToQdrant()
    {
        var client = factory.Services.GetRequiredService<QdrantHttpClient>();
        QdrantHttpResponse<CollectionList> result = await client.GetCollections();
        result.Status.Should().Be("ok");
        var collections = result.Result.Collections;
        collections.Should().NotBeNull();
    }

    [Fact]
    public async Task VerifyAddCollectionToQdrant()
    {
        await CleanupCollections();

        var vectorParams = new VectorParams(4, "Dot", true);

        var client = factory.Services.GetRequiredService<QdrantHttpClient>();

        QdrantHttpResponse<bool> response = await client.CreateCollection(collectionName, vectorParams);
        response.Status.Should().Be("ok");
        response.Should().NotBeNull();

        QdrantHttpResponse<CollectionList> result = await client.GetCollections();
        result.Status.Should().Be("ok");
        var collections = result.Result.Collections;
        collections.Should().NotBeNull();
        collections.Any(c => c.Name == collectionName).Should().BeTrue();
    }




}
