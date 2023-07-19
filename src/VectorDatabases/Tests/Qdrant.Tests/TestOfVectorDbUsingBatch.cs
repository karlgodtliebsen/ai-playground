using System.Text.Json.Serialization;

using AI.Library.Utils;
using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using Microsoft.Extensions.DependencyInjection;

using Qdrant.Tests.Fixtures;

using Xunit.Abstractions;

namespace Qdrant.Tests;

/// <summary>
/// <a href="https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=5ws2UoCZo8bW" >Samples</a>
/// </summary>
[Collection("VectorDb Collection")]
public class TestOfVectorDbUsingBatch
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly QdrantOptions options;
    private readonly JsonSerializerOptions serializerOptions;


    public TestOfVectorDbUsingBatch(VectorDbTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.hostApplicationFactory = fixture.Factory;
        this.options = fixture.Options;
        this.logger = fixture.Logger;
        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
    private const string collectionName = "embeddings-collection";
    private const int vectorSize = 3;


    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information($"{collectionName} deleted", collectionName),
            error => throw new QdrantException(error.Error)
        );
    }


    private async Task CleanUpAndCreateCollectionInVectorDb(int size)
    {
        await CleanupCollection();
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(vectorSize, Distance.DOT, true);
        var client = await qdrantFactory.Create(collectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);
        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded Creating Collection{collectionName}", collectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task AddVectorToCollectionAndUseSearch()
    {

        var vector = new double[][]
        {
          new double[] {0.9, 0.1, 0.1},
          new double[] {0.1, 0.9, 0.1},
          new double[] {0.1, 0.1, 0.9}
        };

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(vectorSize, Distance.DOT, true);
        var client = await qdrantFactory.Create(collectionName, vectorParams, recreateCollection: false, cancellationToken: CancellationToken.None);

        var batch = CreateBatch(vector);
        var payLoad = new BatchUpsertRequest(batch);

        await CleanUpAndCreateCollectionInVectorDb(payLoad.Dimension);

        logger.Information(payLoad.ToJson(serializerOptions));

        var result = await client.Upsert(collectionName, payLoad, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task AddLargeVectorToCollectionAndUseSearch()
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

        var batch = CreateBatch(vector);
        var payLoad = new BatchUpsertRequest(batch);
        logger.Information(payLoad.ToJson(serializerOptions));

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(payLoad.Dimension, Distance.DOT, true);
        var client = await qdrantFactory.Create(collectionName, vectorParams, cancellationToken: CancellationToken.None);

        var result = await client.Upsert(collectionName, payLoad, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    private BatchRequestStruct CreateBatch(double[][] vectors)
    {
        var batch = new BatchRequestStruct();
        uint id = 1;
        for (int i = 0; i < vectors.Length; i++)
        {
            batch.Vectors.Add(vectors[i]);
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
