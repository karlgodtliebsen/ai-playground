using AI.VectorDatabaseQdrant.Configuration;
using Microsoft.Extensions.Options;
using OneOf;
using QdrantCSharp;
using QdrantCSharp.Models;

namespace AI.VectorDatabaseQdrant.VectorStorage;


public class QdrantDb : IVectorDb
{

    private readonly QdrantHttpClient client;
    private readonly QdrantOptions qOptions;

    public QdrantDb(QdrantHttpClient client, IOptions<QdrantOptions> qOptions)
    {
        this.client = client;
        this.qOptions = qOptions.Value;
    }

    public VectorParams CreateParams(int? dimension = null, string? distance = null, bool? storeOnDisk = null)
    {
        if (string.IsNullOrEmpty(distance))
        {
            distance = qOptions.DefaultDistance;
        }
        if (!storeOnDisk.HasValue)
        {
            storeOnDisk = qOptions.DefaultStoreOnDisk;
        }
        if (!dimension.HasValue)
        {
            dimension = qOptions.DefaultDimension;
        }
        var p = new VectorParams(size: dimension.Value, distance: distance, storeOnDisk.Value);
        return p;
    }

    public async Task<OneOf<bool, ErrorResponse>> RemoveAllCollections()
    {
        var allCollection = await GetCollectionNames();
        return await allCollection.Match<Task<OneOf<bool, ErrorResponse>>>(

            async collection =>
            {
                foreach (var collectionName in collection)
                {
                    await RemoveCollection(collectionName);
                }
                return true;
            },
            error =>
                Task.FromResult<OneOf<bool, ErrorResponse>>(
                    new ErrorResponse($"Failed to get collection names: {error.Error}")
                )
        );
    }

    public async Task<OneOf<string, ErrorResponse>> RemoveCollection(string collectionName)
    {
        var result = await client.DeleteCollection(collectionName);
        if (result.Status is not "ok")
        {
            return new ErrorResponse($"Remove collections {collectionName} failed with status {result.Status}.");
        }
        return collectionName;
    }


    public async Task<OneOf<IList<CollectionDescription>, ErrorResponse>> GetCollections()
    {
        // List all the collections
        var collections = await client.GetCollections();
        if (collections.Status is not "ok")
        {
            return new ErrorResponse($"Get collections failed with status {collections.Status}.");
        }

        return collections.Result.Collections.ToList();
    }

    public async Task<OneOf<IList<string>, ErrorResponse>> GetCollectionNames()
    {
        // List all the collections
        var collections = await client.GetCollections();
        if (collections.Status is not "ok")
        {
            return new ErrorResponse($"Get collections failed with status {collections.Status}.");
        }
        return collections.Result.Collections.Select(x => x.Name).ToList();
    }


    public async Task<OneOf<CollectionInfo, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken)
    {
        var existingCollections = await GetCollectionNames();
        return await existingCollections.Match(
           async collection =>
            {
                if (!collection.Contains(collectionName))
                {
                    var result = await client.CreateCollection(collectionName, vectorParams);
                    if (result.Status is not "ok")
                    {
                        return new ErrorResponse($"Create {collectionName} failed with status {result.Status}.");
                    }
                }
                // Get collection info
                var response = await client.GetCollection(collectionName);
                if (response.Status is not "ok")
                {
                    return new ErrorResponse($"GetCollection after Create {collectionName} failed with status {response.Status}.");
                }
                return response.Result;

            },
            error => Task.FromResult<OneOf<CollectionInfo, ErrorResponse>>(
                new ErrorResponse($"Create {collectionName} failed with status {error.Error}."))
        );
    }

    public async Task Upsert(string collectionName, int id, float[] vector, string text, CancellationToken cancellationToken)
    {
        // Insert vectors
        /*await _client.Upsert(collectionName, points: new List<PointStruct>
        {
            new PointStruct(id: id, vector: vector)
        });*/
    }


    public async Task<OneOf<IList<string>, ErrorResponse>> Search(string collectionName, float[] vector, CancellationToken cancellationToken, int limit = 5)
    {
        var result = await client.Search(collectionName, vector, limit);
        return result.Result.Select(x => x.Id.ToString()).ToList();
    }
}
