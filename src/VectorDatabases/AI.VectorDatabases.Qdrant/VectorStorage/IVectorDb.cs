using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using OneOf;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

public interface IVectorDb
{
    VectorParams CreateParams(int? dimension = null, string? distance = null, bool? storeOnDisk = null);

    /// <summary>
    /// a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="vectorParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken);

    /// <summary>
    /// a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="payLoad"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CreateCollectionBody payLoad, CancellationToken cancellationToken);

    /// <summary>
    /// Collection with multiple vectors
    /// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection">Create Collection with multiple vectors</a>
    /// </summary> 
    /// <param name="collectionName"></param>
    /// <param name="payLoad"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CollectCreationBodyWithMultipleNamedVectors payLoad, CancellationToken cancellationToken);



    Task<OneOf<bool, ErrorResponse>> RemoveAllCollections(CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> RemoveCollection(string collectionName, CancellationToken cancellationToken);

    Task<OneOf<CollectionInfo, ErrorResponse>> GetCollection(string collectionName, CancellationToken cancellationToken);

    Task<OneOf<CollectionList, ErrorResponse>> GetCollections(CancellationToken cancellationToken);

    Task<OneOf<IList<string>, ErrorResponse>> GetCollectionNames(CancellationToken cancellationToken);


    //Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken);
    //Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken);
    //Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, BatchStruct batch, CancellationToken cancellationToken);
    //Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, BatchVectors batch, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> DeletePayloadKeys(string collectionName, DeleteFilter filter, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchStruct batch, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchUpsertBody batch, CancellationToken cancellationToken);


    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchStruct batch, CancellationToken cancellationToken);
    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchUpsertBody batch, CancellationToken cancellationToken);


    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, double[] vector, CancellationToken cancellationToken, int limit = 10, int offset = 0);

    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, float[] vector, CancellationToken cancellationToken, int limit = 10, int offset = 0);

    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, SearchBody query, CancellationToken cancellationToken);
}
