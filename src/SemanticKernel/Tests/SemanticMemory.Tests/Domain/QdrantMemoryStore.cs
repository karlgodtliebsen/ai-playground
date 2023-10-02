using System.Runtime.CompilerServices;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Memory;
using OneOf;

namespace SemanticMemory.Tests.Domain;

internal class QdrantMemoryStore : IQdrantMemoryStore
{
    /// <summary>
    /// The Qdrant Vector Database memory store logger.
    /// </summary>
    private readonly ILogger logger;
    private IQdrantClient? qdrantClient = null;

    public QdrantMemoryStore(ILogger logger)
    {
        this.logger = logger;
    }

    public void SetClient(IQdrantClient qdrantVectorDb)
    {
        qdrantClient = qdrantVectorDb;
    }


    /// <inheritdoc/>
    public async Task CreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var exists = await qdrantClient!.DoesCollectionExist(collectionName, cancellationToken).ConfigureAwait(false);
        exists.Switch(
          async result =>
            {
                if (!result)
                {
                    await qdrantClient.CreateCollection(collectionName, cancellationToken).ConfigureAwait(false);
                }
            },
            error => throw new QdrantException("FailedToGetCollections", error.Error)
           );
    }


    /// <inheritdoc/>
    public async Task<bool> DoesCollectionExistAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var exists = await qdrantClient!.DoesCollectionExist(collectionName, cancellationToken).ConfigureAwait(false);
        return exists.Match(
             _ => _,
            error => throw new QdrantException("FailedToGetCollections", error.Error)
        );
    }
    public async IAsyncEnumerable<string> GetCollectionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await qdrantClient!.GetCollections(cancellationToken).ConfigureAwait(false);
        var collectionResult = result.Match(
             c => c.Collections.Select(x => x.Name).ToList(),
             error => throw new QdrantException(error.Error)
         );
        foreach (var s in collectionResult!)
        {
            yield return s;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var exists = await qdrantClient!.DoesCollectionExist(collectionName, cancellationToken).ConfigureAwait(false);
        exists.Switch(
            async doesItExist =>
            {
                if (doesItExist)
                {
                    var result = await qdrantClient.RemoveCollection(collectionName, cancellationToken).ConfigureAwait(false);
                    result.Switch(
                        _ => logger.Information("Deleted Collection: {collectionName}", collectionName),
                        error => throw new QdrantException($"Failed Deleting Collection {collectionName}", error.Error)
                    );
                }
            },
            error => throw new QdrantException($"Failed Finding Collection to Delete {collectionName}", error.Error)
        );

    }

    /// <inheritdoc/>
    public async Task<string> UpsertAsync(string collectionName, MemoryRecord record, CancellationToken cancellationToken = default)
    {
        var vectorData = await ConvertFromMemoryRecordAsync(collectionName, record, cancellationToken).ConfigureAwait(false);
        if (vectorData == null)
        {
            throw new SKException("Failed to convert memory record to Qdrant vector record");
        }
        var batch = CreateBatch(new[] { vectorData });
        var payLoad = new BatchUpsertRequest(batch);

        var result = await qdrantClient!.Upsert(collectionName, payLoad, cancellationToken).ConfigureAwait(false);
        result.Switch(
            _ => logger.Information("UpsertAsync succeeded for: {collectionName}", collectionName),
            error => throw new QdrantException($"Failed to Upsert vectors: {collectionName}", error.Error)
        );
        return vectorData.PointId;
    }

    private BatchRequestStruct CreateBatch(QdrantVectorRecord[] vectors)
    {
        var batch = new BatchRequestStruct();
        for (var i = 0; i < vectors.Length; i++)
        {
            batch.AddToVectors(vectors[i].Embedding);
            batch.Ids.Add(vectors[i].PointId);
            batch.Payloads.Add(vectors[i].Payload);
        }
        return batch;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> UpsertBatchAsync(string collectionName, IEnumerable<MemoryRecord> records,
                                        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var tasks = Task.WhenAll(records.Select(async r => await ConvertFromMemoryRecordAsync(collectionName, r, cancellationToken).ConfigureAwait(false)));
        var vectorData = await tasks.ConfigureAwait(false);

        var batch = CreateBatch(vectorData);
        var payLoad = new BatchUpsertRequest(batch);
        var result = await qdrantClient.Upsert(collectionName, payLoad, cancellationToken).ConfigureAwait(false);
        result.Switch(
            _ => logger.Information("UpsertAsync succeeded for: {collectionName}", collectionName),
            error => throw new QdrantException($"Failed to Upsert vectors: {collectionName}", error.Error)
        );

        foreach (var v in vectorData)
        {
            yield return v.PointId;
        }
    }

    /// <inheritdoc/>
    public async Task<MemoryRecord?> GetAsync(string collectionName, string key, bool withEmbedding = false, CancellationToken cancellationToken = default)
    {
        OneOf<ScoredPoint[], ErrorResponse> vectorDataList = await qdrantClient!.SearchByPayloadId(collectionName, key, withEmbedding, cancellationToken: cancellationToken).ConfigureAwait(false);
        try
        {
            var memoryRecord = vectorDataList.Match(
                points =>
                {
                    var vectorData = points.FirstOrDefault();
                    if (vectorData is null) { return null; }
                    return MemoryRecord.FromJsonMetadata(
                        json: vectorData.GetSerializedPayload(),
                        embedding: vectorData.Vector,
                        key: vectorData.PointId);
                },
                error => throw new QdrantException("GetWithPointIdAsync", error.Error)
            );
            return memoryRecord;
        }
        catch (HttpOperationException ex)
        {
            this.logger.Error(ex, "Failed to get vector data: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<MemoryRecord?> GetAsync2(string collectionName, string key, bool withEmbedding = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var vectorData = await qdrantClient!.GetVectorByIdAsync(collectionName, key, withEmbedding, cancellationToken).ConfigureAwait(false);
            if (vectorData == null) { return null; }

            return MemoryRecord.FromJsonMetadata(
                json: vectorData.GetSerializedPayload(),
                embedding: vectorData.Embedding,
                key: vectorData.PointId);
        }
        catch (HttpOperationException ex)
        {
            this.logger.Error(ex, "Failed to get vector data: {Message}", ex.Message);
            throw;
        }
    }


    /// <inheritdoc/>
    public async IAsyncEnumerable<MemoryRecord> GetBatchAsync(string collectionName, IEnumerable<string> keys, bool withEmbeddings = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        //TODO: needs converting to proper batch
        foreach (var key in keys)
        {
            var record = await GetAsync(collectionName, key, withEmbeddings, cancellationToken).ConfigureAwait(false);
            if (record is not null)
            {
                yield return record;
            }
        }
    }

    /// <summary>
    /// Get a MemoryRecord from the Qdrant Vector database by pointId.
    /// </summary>
    /// <param name="collectionName">The name associated with a collection of embeddings.</param>
    /// <param name="pointId">The unique indexed ID associated with the Qdrant vector record to get.</param>
    /// <param name="withEmbedding">If true, the embedding will be returned in the memory record.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Memory record</returns>
    /// <exception cref="HttpOperationException"></exception>
    public async Task<MemoryRecord?> GetWithPointIdAsync(string collectionName, string pointId, bool withEmbedding = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var vectorDataList = await qdrantClient!.SearchSingleByPointId(collectionName, pointId, withEmbedding, cancellationToken: cancellationToken).ConfigureAwait(false);
            var memoryRecord = vectorDataList.Match<MemoryRecord?>(
                vectorData => MemoryRecord.FromJsonMetadata(json: vectorData.GetSerializedPayload(), embedding: vectorData.Vector),
                _ => null,
                error => throw new QdrantException("GetWithPointIdAsync", error.Error)
             );
            return memoryRecord;
        }
        catch (HttpOperationException ex)
        {
            this.logger.Error(ex, "Failed to get vector data: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Get memory records from the Qdrant Vector database using a group of pointIds.
    /// </summary>
    /// <param name="collectionName">The name associated with a collection of embeddings.</param>
    /// <param name="pointIds">The unique indexed IDs associated with Qdrant vector records to get.</param>
    /// <param name="withEmbeddings">If true, the embeddings will be returned in the memory records.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Memory records</returns>
    public async IAsyncEnumerable<MemoryRecord> GetWithPointIdBatchAsync(string collectionName, IEnumerable<string> pointIds, bool withEmbeddings = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var vectorDataList = await qdrantClient!.SearchByPayloadIds(collectionName, pointIds, withEmbeddings, cancellationToken: cancellationToken);
        var r = vectorDataList.Match(
            p => p,
            error => throw new QdrantException("GetWithPointIdAsync", error.Error)
        );

        foreach (var vectorData in r)
        {
            yield return MemoryRecord.FromJsonMetadata(
                json: vectorData.GetSerializedPayload(),
                embedding: vectorData.Vector,
                key: vectorData.Id);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string collectionName, string key, CancellationToken cancellationToken = default)
    {
        var result = await qdrantClient!.Delete(collectionName, key, cancellationToken).ConfigureAwait(false);

        result.Match(
           _ => _,
           error => throw new QdrantException($"Failed to remove vector data: {collectionName}", error.Error)
       );
    }

    /// <inheritdoc />
    public async Task RemoveBatchAsync(string collectionName, IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(keys.Select(async k => await RemoveAsync(collectionName, k, cancellationToken).ConfigureAwait(false))).ConfigureAwait(false);
    }

    /// <summary>
    /// Remove a MemoryRecord from the Qdrant Vector database by pointId.
    /// </summary>
    /// <param name="collectionName">The name associated with a collection of embeddings.</param>
    /// <param name="pointId">The unique indexed ID associated with the Qdrant vector record to remove.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <exception cref="HttpOperationException"></exception>
    public async Task RemoveWithPointIdAsync(string collectionName, string pointId, CancellationToken cancellationToken = default)
    {

        var result = await qdrantClient!.Delete(collectionName, new[] { pointId }, cancellationToken).ConfigureAwait(false);
        result.Match(
            _ => _,
            error => throw new QdrantException($"Failed to remove vector data: {collectionName}", error.Error)
        );
    }

    /// <summary>
    /// Remove a MemoryRecord from the Qdrant Vector database by a group of pointIds.
    /// </summary>
    /// <param name="collectionName">The name associated with a collection of embeddings.</param>
    /// <param name="pointIds">The unique indexed IDs associated with the Qdrant vector records to remove.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    public async Task RemoveWithPointIdBatchAsync(string collectionName, IEnumerable<string> pointIds, CancellationToken cancellationToken = default)
    {
        var result = await qdrantClient!.Delete(collectionName, pointIds, cancellationToken).ConfigureAwait(false);
        result.Match(
            _ => _,
            error => throw new QdrantException($"Failed to remove vector data: {collectionName}", error.Error)
        );

    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<(MemoryRecord, double)> GetNearestMatchesAsync(
        string collectionName,
        ReadOnlyMemory<float> embedding,
        int limit,
        double minRelevanceScore = 0,
        bool withEmbeddings = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await qdrantClient!.FindNearestInCollection(
              collectionName: collectionName,
              target: embedding.ToArray(),
              threshold: minRelevanceScore,
              top: limit,
              withVectors: withEmbeddings,
              cancellationToken: cancellationToken);

        var scoredPoints = result.Match(
            points => points,
            error => throw new QdrantException("GetNearestMatchesAsync", error.Error)
            );

        foreach (var scoredPoint in scoredPoints)
        {
            yield return (MemoryRecord.FromJsonMetadata(json: scoredPoint.GetSerializedPayload(),
                embedding: scoredPoint.Vector, key: scoredPoint.Id), scoredPoint.Score);
        }
    }

    public async Task<(MemoryRecord, double)?> GetNearestMatchAsync(string collectionName, ReadOnlyMemory<float> embedding, double minRelevanceScore = 0, bool withEmbedding = false, CancellationToken cancellationToken = default)
    {
        var records = GetNearestMatchesAsync(
            collectionName: collectionName,
            embedding: embedding,
            minRelevanceScore: minRelevanceScore,
            limit: 1,
            withEmbeddings: withEmbedding,
            cancellationToken: cancellationToken);
        var record = await records.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        return (record.Item1, record.Item2);
    }


    #region private ================================================================================


    private async Task<QdrantVectorRecord> ConvertFromMemoryRecordAsync(string collectionName, MemoryRecord record, CancellationToken cancellationToken = default)
    {
        string pointId;

        // Check if a database key has been provided for update
        if (!string.IsNullOrEmpty(record.Key))
        {
            pointId = record.Key;
        }
        // Check if the data store contains a record with the provided metadata ID
        else
        {
            var existingRecord = await qdrantClient.SearchSingleByPayloadId(collectionName, record.Metadata.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
            pointId = await existingRecord.Match<Task<string>>(
                point => Task.FromResult(point.PointId), // similar to existingRecord.PointId;
                async _ =>
                {
                    var @break = false;
                    string pid;
                    do  // Generate a new ID until a unique one is found (more than one pass should be exceedingly rare)
                    {
                        // If no matching record can be found, generate an ID for the new record
                        pid = Guid.NewGuid().ToString();
                        existingRecord = await qdrantClient.SearchSingleByPointId(collectionName, pid, cancellationToken: cancellationToken);
                        existingRecord.Switch(
                            _ => { },
                            _ => @break = true,
                            error => throw new QdrantException(error.Error)
                        );

                    } while (!@break);
                    return pid;
                },
                error => throw new SKException("Failed to convert memory record to Qdrant vector record")
            );
        }

        var vectorData = QdrantVectorRecord.FromJsonMetadata(pointId: pointId, embedding: record.Embedding, json: record.GetSerializedMetadata());

        if (vectorData is null)
        {
            throw new SKException("Failed to convert memory record to Qdrant vector record");
        }

        return vectorData;
    }

    #endregion
}
