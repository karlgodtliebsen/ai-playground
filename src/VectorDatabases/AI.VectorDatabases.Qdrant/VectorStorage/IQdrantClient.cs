using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using OneOf;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

public interface IQdrantClient
{
    void SetCollectionName(string collectionName);

    void SetVectorSize(int dimension);

    void UseParams(VectorParams @params);


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
    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CreateCollectionWithVectorRequest payLoad, CancellationToken cancellationToken);

    /// <summary>
    /// a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CancellationToken cancellationToken);

    /// <summary>
    /// Collection with multiple vectors
    /// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection">Create Collection with multiple vectors</a>
    /// </summary> 
    /// <param name="collectionName"></param>
    /// <param name="payLoad"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CreateCollectionWithMultipleNamedVectorsRequest payLoad, CancellationToken cancellationToken);


    Task<OneOf<bool, ErrorResponse>> DoesCollectionExist(string collectionName, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> RemoveAllCollections(CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> RemoveCollection(string collectionName, CancellationToken cancellationToken);

    Task<OneOf<CollectionInfo, ErrorResponse>> GetCollection(string collectionName, CancellationToken cancellationToken);
    Task<OneOf<CollectionList, ErrorResponse>> GetCollections(CancellationToken cancellationToken);
    Task<OneOf<IList<string>, ErrorResponse>> GetCollectionNames(CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IEnumerable<string> pointIds, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IEnumerable<PointStruct> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IEnumerable<PointStructWithNamedVector> points, CancellationToken cancellationToken);
    Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, string payloadId, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> DeletePayloadKeys(string collectionName, DeleteFilter filter, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchRequestStruct batchRequest, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchUpsertRequest batch, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken);

    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchRequestStruct batchRequest, CancellationToken cancellationToken);
    Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchUpsertRequest batch, CancellationToken cancellationToken);


    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, double[] vector, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, float[] vector, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

    Task<OneOf<ScoredPoint[], ErrorResponse>> SearchByPayloadIds(string collectionName, IEnumerable<string> ids, bool withVectors = false, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);
    Task<OneOf<ScoredPoint[], ErrorResponse>> SearchByPayloadId(string colName, string id, bool withVectors = false, CancellationToken cancellationToken = default);

    Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, SearchRequest query, CancellationToken cancellationToken);
    Task<OneOf<ScoredPoint[], ErrorResponse>> SearchByPointIds(string collectionName, IEnumerable<string> pointIds, bool withVectors = false, CancellationToken cancellationToken = default);

    Task<OneOf<ScoredPoint, NullResult, ErrorResponse>> SearchSingleByPayloadId(string collectionName, string id, bool withVectors = false, CancellationToken cancellationToken = default);
    Task<OneOf<ScoredPoint, NullResult, ErrorResponse>> SearchSingleByPointId(string collectionName, string pointId, bool withVectors = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find the nearest vectors in a collection using vector similarity search.
    /// </summary>
    /// <param name="collectionName">The name assigned to a collection of vectors.</param>
    /// <param name="target">The vector to compare the collection's vectors with.</param>
    /// <param name="threshold">The minimum relevance threshold for returned results.</param>
    /// <param name="top">The maximum number of similarity results to return.</param>
    /// <param name="withVectors">Whether to include the vector data in the returned results.</param>
    /// <param name="requiredTags">Qdrant tags used to filter the results.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    public Task<OneOf<ScoredPoint[], ErrorResponse>> FindNearestInCollection(string collectionName, IEnumerable<float> target, double threshold, int top = 1, bool withVectors = false, IEnumerable<string>? requiredTags = null, CancellationToken cancellationToken = default);



    /// <summary>
    /// Get a specific vector by a unique identifier in the metadata (Qdrant payload).
    /// </summary>
    /// <param name="collectionName">The name assigned to a collection of vectors.</param>
    /// <param name="pointId">The unique ID stored in a Qdrant vector entry's metadata.</param>
    /// <param name="withVector">Whether to include the vector data in the returned result.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The Qdrant vector record associated with the given ID if found, null if not.</returns>
    public Task<VectorRecord?> GetVectorByIdAsync(string collectionName, string pointId, bool withVector = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get vectors by their unique Qdrant pointIds.
    /// </summary>
    /// <param name="collectionName">The name assigned to the collection of vectors.</param>
    /// <param name="pointIds">The unique IDs used to index Qdrant vector entries.</param>
    /// <param name="withVectors">Whether to include the vector data in the returned results.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>An asynchronous list of Qdrant vectors records associated with the given IDs</returns>
    public IAsyncEnumerable<VectorRecord> GetVectorsByIdAsync(string collectionName, IEnumerable<string> pointIds, bool withVectors = false, CancellationToken cancellationToken = default);

}

public class NullResult
{

}
