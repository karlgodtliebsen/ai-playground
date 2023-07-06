using OneOf;

using QdrantCSharp.Models;

namespace AI.Library.Qdrant.VectorStorage;

public interface IVectorDb
{
    VectorParams CreateParams(int? dimension = null, string? distance = null, bool? storeOnDisk = null);

    Task<OneOf<bool, ErrorResponse>> RemoveAllCollections();

    Task<OneOf<string, ErrorResponse>> RemoveCollection(string collectionName);


    Task<OneOf<IList<CollectionDescription>, ErrorResponse>> GetCollections();

    Task<OneOf<IList<string>, ErrorResponse>> GetCollectionNames();


    Task<OneOf<CollectionInfo, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken);


    Task Upsert(string collectionName, int id, float[] vector, string text, CancellationToken cancellationToken);

    Task<OneOf<IList<string>, ErrorResponse>> Search(string collectionName, float[] vector, CancellationToken cancellationToken, int limit = 5);
}
