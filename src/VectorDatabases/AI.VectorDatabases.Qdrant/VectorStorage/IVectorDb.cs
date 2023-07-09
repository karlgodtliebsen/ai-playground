using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;
using OneOf;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

public interface IVectorDb
{
    VectorParams CreateParams(int? dimension = null, string? distance = null, bool? storeOnDisk = null);


    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken);


    Task<OneOf<bool, ErrorResponse>> RemoveAllCollections(CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> RemoveCollection(string collectionName, CancellationToken cancellationToken);

    Task<OneOf<CollectionInfo, ErrorResponse>> GetCollection(string collectionName, CancellationToken cancellationToken);

    Task<OneOf<CollectionList, ErrorResponse>> GetCollections(CancellationToken cancellationToken);

    Task<OneOf<IList<string>, ErrorResponse>> GetCollectionNames(CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken);

    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, float[] vector, CancellationToken cancellationToken, int limit = 10, int offset = 0);

    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, SearchBody query, CancellationToken cancellationToken);
}
