using AI.VectorDatabaseQdrant.Configuration;
using AI.VectorDatabaseQdrant.VectorStorage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Qdrant.Tests.Utils;

using Xunit.Abstractions;

namespace Qdrant.Tests;


public class TestOfVectorDb
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly QdrantOptions options;

    public TestOfVectorDb(ITestOutputHelper output)
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
        var result = await client.RemoveAllCollections();
        result.Switch(

            _ => output.WriteLine("All collections deleted"),
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task VerifyInitialAccessToVectorDb()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.GetCollections();
        result.Switch(

            collections => output.WriteLine(string.Join("|", collections.Select(c => c.Name))),
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task VerifyCreateCollectionInVectorDb()
    {
        await CleanupCollections();
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var vectorParams = client.CreateParams(4, "Dot", true);
        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            collectionInfo => output.WriteLine(collectionInfo.Status),
            error => throw new QdrantException(error.Error)
        );
    }
}
