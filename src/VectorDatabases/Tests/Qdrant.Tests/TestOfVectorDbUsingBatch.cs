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
/// <a href="https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=5ws2UoCZo8bW" />
/// </summary>
[Collection("VectorDb Collection")]
public class TestOfVectorDbUsingBatch
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly QdrantOptions options;
    private readonly JsonSerializerOptions serializerOptions;


    public TestOfVectorDbUsingBatch(VectorDbTestFixture fixture, ITestOutputHelper output)
    {
        fixture.GetOutput = () => output;
        this.output = output;
        this.factory = fixture.Factory;
        this.options = fixture.Options;
        this.logger = fixture.Logger;
        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        LaunchQdrantDocker.Launch();
    }
    private const string collectionName = "embeddings-collection";


    private async Task CleanupCollection()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine($"{collectionName} deleted"),
            error => throw new QdrantException(error.Error)
        );
    }


    private async Task CleanUpAndCreateCollectionInVectorDb(int size)
    {
        await CleanupCollection();

        var client = factory.Services.GetRequiredService<IVectorDb>();
        var vectorParams = client.CreateParams(size, Distance.COSINE, true);
        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
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
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var batch = CreateBatch(vector);
        var payLoad = new BatchUpsertBody(batch);

        await CleanUpAndCreateCollectionInVectorDb(payLoad.Dimension);

        output.WriteLine(payLoad.ToJson(serializerOptions));

        var result = await client.Upsert(collectionName, payLoad, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
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

        var client = factory.Services.GetRequiredService<IVectorDb>();
        var batch = CreateBatch(vector);
        var payLoad = new BatchUpsertBody(batch);
        output.WriteLine(payLoad.ToJson(serializerOptions));

        await CleanUpAndCreateCollectionInVectorDb(payLoad.Dimension);
        var result = await client.Upsert(collectionName, payLoad, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    private BatchStruct CreateBatch(double[][] vectors)
    {
        var ids = new List<object>(vectors.Length);
        for (int i = 0; i < vectors.Length; i++)
        {
            ids.Add(i);
        }

        var payloads = new List<Dictionary<string, string>>(vectors.Length);
        for (int i = 0; i < vectors.Length; i++)
        {
            payloads.Add(new Dictionary<string, string>()
            {
                {"color", "red " + i}
            });
        }

        var batch = new BatchStruct()
        {
            Ids = ids.ToArray(),
            Vectors = vectors,
            Payloads = payloads.ToArray()
        };

        return batch;
    }
}
