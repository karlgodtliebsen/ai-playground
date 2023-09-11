using AI.Library.Utils;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Qdrant.Tests.Fixtures;

using Xunit.Abstractions;

namespace Qdrant.Tests;

/// <summary>
/// <a href="https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=5ws2UoCZo8bW" >Samples</a>
/// </summary>
[Collection("VectorDb Collection")]
public class TestOfVectorDbUsingBatch : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly QdrantOptions options;
    private readonly IServiceProvider services;
    private readonly VectorDbTestFixture fixture;


    private const string CollectionName = "embeddings-collection";
    private const int VectorSize = 3;
    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    public TestOfVectorDbUsingBatch(VectorDbTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.options = services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        this.logger = services.GetRequiredService<ILogger>();
    }


    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantClient>();
        var result = await client.RemoveCollection(CollectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information($"{CollectionName} deleted", CollectionName),
            error => throw new QdrantException(error.Error)
        );
    }


    private async Task CleanUpAndCreateCollectionInVectorDb(int size)
    {
        await CleanupCollection();
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(VectorSize, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);
        var result = await client.CreateCollection(CollectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded Creating Collection{collectionName}", CollectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task AddVectorToCollectionAndUseSearch()
    {

        var vector = new ReadOnlyMemory<float>[]
        {
          new ReadOnlyMemory<float>(new float[] {0.9f, 0.1f, 0.1f}),
          new ReadOnlyMemory<float>(new float[] {0.1f, 0.9f, 0.1f}),
          new ReadOnlyMemory<float>(new float[] {0.1f, 0.1f, 0.9f}),
        };

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(VectorSize, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);

        var batch = CreateBatch(vector);
        var payLoad = new BatchUpsertRequest(batch);

        await CleanUpAndCreateCollectionInVectorDb(payLoad.Dimension);

        logger.Information(payLoad.ToJson(DefaultJsonSerializerOptions.DefaultOptions));

        var result = await client.Upsert(CollectionName, payLoad, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }


    public double[][] GetLargeVector()
    {

        //https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=5ws2UoCZo8bW
        var vector = new double[][]
        {
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
            new double[] { -0.05303611,  0.34459755,  0.76877484, -0.0158912 , -0.3515725 , -0.92520697, -0.36416004,  0.91791994, -0.2254738 ,  0.03992614, -0.17834748, -0.58472613, -0.89322339,  0.13848185, -0.90751362, 0.71058809, -0.27512001,  0.64711605,  0.30991896,  0.8896701 },
            new double[] {-0.1296899 , -0.8325752 ,  0.46608321, -0.39436982,  0.12301721, 0.22336377, -0.95403339,  0.30383946,  0.7568641 , -0.91504574, 0.21398519, -0.43977382, -0.07772702,  0.02275247, -0.22655445, -0.02363874, -0.56423764,  0.94943287,  0.26219995,  0.62735642},
        };
        return vector;
    }

    [Fact]
    public void SerializeAndDeserializeLargeVectorUsingBatchUpsertRequest()
    {
        var vector = GetLargeVector();
        ReadOnlyMemory<float>[] vectors = new ReadOnlyMemory<float>[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            vectors[i] = new ReadOnlyMemory<float>(vector[i].Select(e => (float)e).ToArray());
        }

        var batch = CreateBatch(vectors);
        var payLoad = new BatchUpsertRequest(batch);

        var s = JsonSerializer.Serialize(payLoad, DefaultJsonSerializerOptions.DefaultOptions);
        var payLoad2 = JsonSerializer.Deserialize<BatchUpsertRequest>(s, DefaultJsonSerializerOptions.DefaultOptions);
        Assert.Equal(payLoad2.Dimension, payLoad.Dimension);
    }

    [Fact]
    public void SerializeAndDeserializePointStruct()
    {

        //https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=5ws2UoCZo8bW

        var pointStruct = new PointStruct()
        {
            Id = "42",
            Vector = new ReadOnlyMemory<float>(new float[]
            {
                0.9f, 0.1f, 0.1f
            }),
            Payload = new Dictionary<string, string>()
            {
                {"42", "42"}
            }
        };

        var s = JsonSerializer.Serialize(pointStruct, DefaultJsonSerializerOptions.DefaultOptions);
        var pointStruct2 = JsonSerializer.Deserialize<PointStruct>(s, DefaultJsonSerializerOptions.DefaultOptions);
        Assert.Equal(pointStruct2.Id, pointStruct.Id);
        Assert.Equal(pointStruct2.Vector!.Value.ToArray().First(), pointStruct.Vector!.Value.ToArray().First());
    }



    [Fact]
    public async Task AddLargeVectorToCollectionAndUseSearch()
    {

        //https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=5ws2UoCZo8bW

        var vector = GetLargeVector();

        ReadOnlyMemory<float>[] vectors = new ReadOnlyMemory<float>[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            vectors[i] = new ReadOnlyMemory<float>(vector[i].Select(e => (float)e).ToArray());
        }

        var batch = CreateBatch(vectors);
        var payLoad = new BatchUpsertRequest(batch);
        logger.Information(payLoad.ToJson(DefaultJsonSerializerOptions.DefaultOptions));

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(payLoad.Dimension, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, cancellationToken: CancellationToken.None);

        var result = await client.Upsert(CollectionName, payLoad, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    private BatchRequestStruct CreateBatch(ReadOnlyMemory<float>[] vectors)
    {
        var batch = new BatchRequestStruct();
        uint id = 1;
        for (int i = 0; i < vectors.Length; i++)
        {
            batch.AddToVectors(vectors[i]);
            batch.Ids.Add(Guid.NewGuid().ToString());
            batch.Payloads.Add(new Dictionary<string, object>()
            {
                {"color", "red " +(id)}
            });
            id++;
        }
        return batch;
    }
}
