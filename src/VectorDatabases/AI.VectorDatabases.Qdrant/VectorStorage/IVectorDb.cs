using AI.VectorDatabaseQdrant.VectorStorage.Models;

using OneOf;

//using CollectionInfo = AI.VectorDatabaseQdrant.VectorStorage.Models.CollectionInfo;
//using CollectionList = AI.VectorDatabaseQdrant.VectorStorage.Models.CollectionList;
//using PointStruct = AI.VectorDatabaseQdrant.VectorStorage.Models.PointStruct;
//using ScoredPoint = AI.VectorDatabaseQdrant.VectorStorage.Models.ScoredPoint;
//using VectorParams = AI.VectorDatabaseQdrant.VectorStorage.Models.VectorParams;

namespace AI.VectorDatabaseQdrant.VectorStorage;

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
}
