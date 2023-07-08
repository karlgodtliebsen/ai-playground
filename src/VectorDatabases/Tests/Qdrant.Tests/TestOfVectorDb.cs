using AI.VectorDatabaseQdrant.Configuration;
using AI.VectorDatabaseQdrant.VectorStorage;
using AI.VectorDatabaseQdrant.VectorStorage.Models;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Qdrant.Tests.Utils;


using Xunit.Abstractions;

using Distance = AI.VectorDatabaseQdrant.VectorStorage.Models.Distance;
using PointStruct = AI.VectorDatabaseQdrant.VectorStorage.Models.PointStruct;

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
    public async Task VerifyInitialAccessToVectorDb()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.GetCollections(CancellationToken.None);
        result.Switch(

            collectionList => output.WriteLine(string.Join("|", collectionList.Collections.Select(c => c.Name))),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task CleanupCollections()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.RemoveAllCollections(CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("All collections deleted"),
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task VerifyCreateCollectionInVectorDb()
    {
        await CleanupCollections();
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var vectorParams = client.CreateParams(4, Distance.DOT, true);
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

    [Fact]
    public async Task AddVectorToCollections()
    {
        await VerifyCreateCollectionInVectorDb();
        await Task.Delay(1000);

        var client = factory.Services.GetRequiredService<IVectorDb>();

        //float[] vector = new float[] { 1, 2, 3, 4/*, 5, 6, 7, 8, 9, 10 */};
        //string text = Distance.DOT;
        //[
        //{ "id": 1, "vector": [0.05, 0.61, 0.76, 0.74], "payload": { "city": "Berlin"} },
        //{ "id": 2, "vector": [0.19, 0.81, 0.75, 0.11], "payload": { "city": ["Berlin", "London"] } },
        //{ "id": 3, "vector": [0.36, 0.55, 0.47, 0.94], "payload": { "city": ["Berlin", "Moscow"] } },
        //{ "id": 4, "vector": [0.18, 0.01, 0.85, 0.80], "payload": { "city": ["London", "Moscow"] } },
        //{ "id": 5, "vector": [0.24, 0.18, 0.22, 0.44], "payload": { "count": [0]} },
        //{ "id": 6, "vector": [0.35, 0.08, 0.11, 0.44]}
        //]
        //"[{\"id\":1,\"vector\":[0.05,0.61,0.76,0.74],\"payload\":{\"city\":\"Berlin\"}},{\"id\":2,\"vector\":[0.19,0.81,0.75,0.11],\"payload\":{\"city\":[\"Berlin\",\"London\"]}},{\"id\":3,\"vector\":[0.36,0.55,0.47,0.94],\"payload\":{\"city\":[\"Berlin\",\"Moscow\"]}},{\"id\":4,\"vector\":[0.18,0.01,0.85,0.8],\"payload\":{\"city\":[\"London\",\"Moscow\"]}},{\"id\":5,\"vector\":[0.24,0.18,0.22,0.44],\"payload\":{\"count\":[0]}},{\"id\":6,\"vector\":[0.35,0.08,0.11,0.44]}]"
        IList<PointStruct> points = new List<PointStruct>()
        {

            new PointStruct()
            {
                Id = 1,
                Vector = new float[] { 0.05f, 0.61f, 0.76f, 0.74f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", "Berlin" }
                }
            },
            new PointStruct()
            {
                Id = 2,
                Vector = new float[] { 0.19f, 0.81f, 0.75f, 0.11f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "Berlin", "London" } }
                }
            },
            new PointStruct()
            {
                Id = 3,
                Vector = new float[] { 0.36f, 0.55f, 0.47f, 0.94f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "Berlin", "Moscow" } }
                }
            },
            new PointStruct()
            {
                Id = 4,
                Vector = new float[] { 0.18f, 0.01f, 0.85f, 0.80f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "London", "Moscow" } }
                }
            },
            new PointStruct()
            {
                Id = 5,
                Vector = new float[] { 0.24f, 0.18f, 0.22f, 0.44f },
                Payload = new Dictionary<string, object>()
                {
                    { "count", new int[] { 0 } }
                }
            },
            new PointStruct()
            {
                Id = 6,
                Vector = new float[] { 0.35f, 0.08f, 0.11f, 0.44f }
            }
        };

        var result = await client.Upsert(collectionName, points, CancellationToken.None);

        result.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );

        var vector = new float[]
        {
            0.05f, 0.61f, 0.76f, 0.74f
        };

        var searchResult = await client.Search(collectionName, vector, CancellationToken.None);
        searchResult.Switch(

            r =>
            {
                output.WriteLine("Result: " + r.First(x => x.Score > 1.0).Score);
            },
            error => throw new QdrantException(error.Error)
        );
    }
}
