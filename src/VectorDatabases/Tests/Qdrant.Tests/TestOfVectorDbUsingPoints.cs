using System.Text.Json.Serialization;

using AI.Library.Utils;
using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Qdrant.Tests.Fixtures;

using Xunit.Abstractions;

namespace Qdrant.Tests;

[Collection("VectorDb Collection")]
public class TestOfVectorDbUsingPoints
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly QdrantOptions options;
    private readonly JsonSerializerOptions serializerOptions;

    public TestOfVectorDbUsingPoints(VectorDbTestFixture fixture, ITestOutputHelper output)
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


    private const string collectionName = "test-collection";


    private async Task CleanupCollection()
    {
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine($"{collectionName} deleted"),
            error => throw new QdrantException(error.Error)
        );
    }

    private async Task CleanUpAndCreateCollectionInVectorDb()
    {
        await CleanupCollection();
        await Task.Delay(1000);

        var client = factory.Services.GetRequiredService<IVectorDb>();
        var vectorParams = client.CreateParams(4, Distance.DOT, true);
        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task AddVectorToCollectionAndUseSearch()
    {
        await CleanUpAndCreateCollectionInVectorDb();
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var points = CreatePoints();
        var result = await client.Upsert(collectionName, points, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );

        var vector = new double[]
        {
            0.05f, 0.61f, 0.76f, 0.74f
        };

        var payLoad = new SearchBody()
        {
            Limit = 10,
            Offset = 0,
        };
        payLoad.SetVector(vector);
        output.WriteLine(payLoad.ToJson(serializerOptions));

        var searchResult = await client.Search(collectionName, vector, CancellationToken.None);
        searchResult.Switch(

            res =>
            {
                res.Length.Should().Be(7);
                foreach (var r in res)
                {
                    output.WriteLine($"Result: {r.Payload.ToJson()} {+r.Score} {r.Id}");
                }
            },
            error => throw new QdrantException(error.Error)
        );
    }

    public async Task AddBasicDataToCollection()
    {
        await CleanUpAndCreateCollectionInVectorDb();
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var points = CreatePoints();
        var result = await client.Upsert(collectionName, points, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task AddVectorToCollectionAndUseSearchWithPayload()
    {
        await AddBasicDataToCollection();
        var client = factory.Services.GetRequiredService<IVectorDb>();

        var vector = new double[]
        {
            0.05f, 0.61f, 0.76f, 0.74f
        };

        var search = new SearchBody();
        search.SetVector(vector);
        search.SetWithPayload(true);

        var searchResult = await client.Search(collectionName, search, CancellationToken.None);
        searchResult.Switch(

            res =>
            {
                res.Length.Should().Be(7);
                foreach (var r in res)
                {
                    output.WriteLine($"Result: {r.Payload!.ToJson()} {+r.Score} {r.Id}");
                }
            },
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task AddVectorToCollectionAndUseSearchWithMustFilter()
    {
        await AddBasicDataToCollection();
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var vector = new double[]
        {
            0.05f, 0.61f, 0.76f, 0.74f
        };

        var search = new SearchBody();
        search.SetVector(vector);
        search.SetWithPayload(true);

        search.Filter = new SearchFilter() { };
        search.Filter.AddMust(new ConditionalFilter
        {
            Key = "city",
            Match = new MatchFilter()
            {
                Value = "Berlin"
            }
        }
        );

        var serialized = JsonSerializer.Serialize(search, serializerOptions);
        output.WriteLine(serialized);

        var searchResult = await client.Search(collectionName, search, CancellationToken.None);
        searchResult.Switch(

            res =>
            {
                res.Length.Should().Be(4);
                foreach (var r in res)
                {
                    output.WriteLine($"Result: {r.Payload.ToJson()} {+r.Score} {r.Id}");
                }
            },
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact]
    public async Task AddVectorToCollectionAndUseSearchWithMustNotFilter()
    {
        await AddBasicDataToCollection();
        var client = factory.Services.GetRequiredService<IVectorDb>();
        var vector = new double[]
        {
            0.05f, 0.61f, 0.76f, 0.74f
        };

        var search = new SearchBody();
        search.SetVector(vector);
        search.SetWithPayload(true);
        search.Filter = new SearchFilter() { };
        search.Filter.AddMustNot(new ConditionalFilter
        {
            Key = "city",
            Match = new MatchFilter()
            {
                Value = "Berlin"
            }
        }
        );

        var serialized = JsonSerializer.Serialize(search, serializerOptions);
        output.WriteLine(serialized);

        var searchResult = await client.Search(collectionName, search, CancellationToken.None);
        searchResult.Switch(

            res =>
            {
                res.Length.Should().Be(3);
                foreach (var r in res)
                {
                    output.WriteLine($"Result: {r.Payload.ToJson()} {+r.Score} {r.Id}");
                }
            },
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task AddDataWithNamedVectorsToCollectionAndUseSearch()
    {
        await CleanupCollection();

        var client = factory.Services.GetRequiredService<IVectorDb>();

        var vectors = new CollectCreationBodyWithMultipleNamedVectors();
        vectors.NamedVectors = new Dictionary<string, VectorParams>()
        {
            {"image", new VectorParams(4,Distance.DOT)} ,
            {"text", new VectorParams(8,Distance.COSINE)} ,
        };
        output.WriteLine(vectors.ToJson(serializerOptions));

        var result1 = await client.CreateCollection(collectionName, vectors, CancellationToken.None);
        result1.Switch(

            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );


        var pointsWithNamedVectors = CreatePointsWithNames();
        var body = new PointsWithNamedVectorsUpsertBody()
        {
            Points = pointsWithNamedVectors.ToArray()
        };

        output.WriteLine("");
        output.WriteLine(body.ToJson(serializerOptions));

        var result = await client.Upsert(collectionName, pointsWithNamedVectors, CancellationToken.None);
        result.Switch(
            _ => output.WriteLine("Succeeded"),
            error => throw new QdrantException(error.Error)
        );

        var vector = new double[]
        {
            0.9, 0.1, 0.1, 0.2
        };

        var search = new SearchBody();
        search.Limit = 3;
        var p = new Dictionary<string, object>()
                {
                    {"name", "image"},
                    {"vector", vector}
                };

        search.SetVector(p);
        output.WriteLine("");
        output.WriteLine(search.ToJson(serializerOptions));

        var searchResult = await client.Search(collectionName, search, CancellationToken.None);
        searchResult.Switch(
            res =>
            {
                res.Length.Should().Be(2);
                foreach (var r in res)
                {
                    output.WriteLine($"Result: {+r.Score} {r.Id}");
                }
            },
            error => throw new QdrantException(error.Error)
        );
    }


    //https://qdrant.github.io/qdrant/redoc/index.html#tag/points/operation/search_points
    private IList<PointStruct> CreatePoints()
    {
        IList<PointStruct> points = new List<PointStruct>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.05f, 0.61f, 0.76f, 0.74f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", "Berlin" }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.19f, 0.81f, 0.75f, 0.11f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "Berlin", "London" } }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.36f, 0.55f, 0.47f, 0.94f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "Berlin", "Rome" } }
                }
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.18f, 0.01f, 0.85f, 0.80f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "London", "Rome" } }
                }
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.24f, 0.18f, 0.22f, 0.44f },
                Payload = new Dictionary<string, object>()
                {
                    { "count", new int[] { 0 } }
                }
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.35f, 0.08f, 0.11f, 0.44f }
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Vector = new double[] { 0.35f, 0.08f, 0.11f, 0.44f },
                Payload = new Dictionary<string, object>()
                {
                    { "city", new string[] { "Berlin", "London" } }
                }
            }

        };
        return points;
    }

    //private IList<PointStruct> CreateImageTextPoints()
    //{
    //    IList<PointStruct> points = new List<PointStruct>()
    //    {
    //        new()
    //        {
    //            Id = Guid.NewGuid(),
    //            Vector = new double[] { 0.05f, 0.61f, 0.76f, 0.74f },
    //            Payload = new Dictionary<string, object>()
    //            {
    //                { "image", "text" }
    //            }
    //        },
    //         new()
    //        {
    //            Id = Guid.NewGuid(),
    //            Vector = new double[] { 0.2, 0.1, 0.3, 0.9 },
    //            Payload = new Dictionary<string, object>()
    //            {
    //                { "image", "text" }
    //            }
    //        },

    //         new()
    //         {
    //             Id =Guid.NewGuid(),
    //             Vector = new double[] { 0.9, 0.1, 0.1, 0.2 },
    //             Payload = new Dictionary<string, object>()
    //             {
    //                 { "image", "text" }
    //             }
    //         },
    //    };
    //    return points;
    //}

    /*
     {
    "points": [
        {
            "id": 1,
            "vector": {
                "image": [0.9, 0.1, 0.1, 0.2],
                "text": [0.4, 0.7, 0.1, 0.8, 0.1, 0.1, 0.9, 0.2]
            }
        },
        {
            "id": 2,
            "vector": {
                "image": [0.2, 0.1, 0.3, 0.9],
                "text": [0.5, 0.2, 0.7, 0.4, 0.7, 0.2, 0.3, 0.9]
            }
        }
    ]
}

    */

    private IList<PointStructWithNamedVector> CreatePointsWithNames()
    {
        var points = new List<PointStructWithNamedVector>()
        {
            new ()//If the collection was created with multiple vectors, each vector data can be provided using the vector’s name:
            {
                Id = Guid.NewGuid(),
                Vector =
                    new Dictionary<string, double[]>()
                    {
                        {  "image", new double[] { 0.9, 0.1, 0.1, 0.2 }},
                        {  "text", new double[] { 0.4, 0.7, 0.1, 0.8, 0.1, 0.1, 0.9, 0.2 }}
                    }
               }
            ,
            new ()
            {

                Id = Guid.NewGuid(),
                Vector =
                    new Dictionary<string, double[]>()
                    {
                        {  "image", new double[] { 0.2, 0.1, 0.3, 0.9 }},
                        {  "text", new double[] { 0.5, 0.2, 0.7, 0.4, 0.7, 0.2, 0.3, 0.9}}
                    }
            }
        };
        return points;
    }
}

