using System.Runtime.CompilerServices;

using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.Diagnostics;
using Microsoft.SemanticMemory.MemoryStorage;
using Microsoft.SemanticMemory.MemoryStorage.Qdrant.Client;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain.QdrantFactory;

public class QdrantMemoryStoreForSemanticMemory : ISemanticMemoryVectorDb
{

    private IQdrantClient? qdrantClient;
    private readonly IQdrantFactory qdrantFactory;
    private readonly ILogger<QdrantMemoryStoreForSemanticMemory> _log;


    public QdrantMemoryStoreForSemanticMemory(IQdrantFactory qdrantFactory, ILogger<QdrantMemoryStoreForSemanticMemory>? log = null)
    {
        this.qdrantFactory = qdrantFactory;
        _log = log ?? DefaultLogger<QdrantMemoryStoreForSemanticMemory>.Instance;
    }

    /// <inheritdoc />
    public async Task CreateIndexAsync(string indexName, int vectorSize, CancellationToken cancellationToken = default)
    {
        if (qdrantClient is null)
        {
            var vectorParams = qdrantFactory.CreateParams(vectorSize, Distance.DOT, true);
            qdrantClient = await qdrantFactory.Create(indexName, vectorParams, recreateCollection: true, storeOnDisk: true, cancellationToken: cancellationToken);
            await qdrantClient.CreateCollection(indexName, vectorParams, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        indexName = NormalizeIndexName(indexName);
        if (string.Equals(indexName, Constants.DefaultIndex, StringComparison.OrdinalIgnoreCase))
        {
            _log.LogWarning("The default index cannot be deleted");
            return;
        }
        await qdrantClient!.RemoveCollection(indexName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> UpsertAsync(string collectionName, MemoryRecord record, CancellationToken cancellationToken = default)
    {
        collectionName = NormalizeIndexName(collectionName);

        QdrantPoint<DefaultQdrantPayload> qdrantPoint;

        if (string.IsNullOrEmpty(record.Id))
        {
            record.Id = Guid.NewGuid().ToString("N");
            qdrantPoint = QdrantPoint<DefaultQdrantPayload>.FromMemoryRecord(record);
            qdrantPoint.Id = Guid.NewGuid();
            _log.LogTrace("Generate new Qdrant point ID {0} and record ID {1}", qdrantPoint.Id, record.Id);
        }
        else
        {
            qdrantPoint = QdrantPoint<DefaultQdrantPayload>.FromMemoryRecord(record);
            var existingVector =
                await qdrantClient!.GetVectorByIdAsync(collectionName, record.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (existingVector is null)
            {
                qdrantPoint.Id = Guid.NewGuid();
                _log.LogTrace("No record with ID {0} found, generated a new point ID {1}", record.Id, qdrantPoint.Id);
            }
            else
            {
                qdrantPoint.Id = Guid.Parse(existingVector.PointId);
                _log.LogTrace("Point ID {0} found, updating...", qdrantPoint.Id);
            }
        }
        IList<PointStruct> points = new List<PointStruct>
            {
                new PointStruct
                {
                    Id = qdrantPoint.Id.ToString("N"),
                    Payload = qdrantPoint.Payload,
                    Vector = qdrantPoint.Vector.Data.ToArray()
                }
            };
        await qdrantClient!.Upsert(collectionName, points, cancellationToken).ConfigureAwait(false);

        return record.Id;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<(MemoryRecord, double)> GetSimilarListAsync(string collectionName, Embedding embedding, int limit, double minRelevanceScore = 0, MemoryFilter? filter = null, bool withEmbeddings = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        collectionName = NormalizeIndexName(collectionName);

        if (limit <= 0)
        {
            limit = int.MaxValue;
        }

        var requiredTags = filter != null && !filter.IsEmpty()
            ? filter.GetFilters().Select(x => $"{x.Key}{Constants.ReservedEqualsSymbol}{x.Value}")
            : new List<string>();

        var results = await qdrantClient!.FindNearestInCollection(
                                            collectionName: collectionName,
                                            target: embedding.Data.ToArray(),
                                            threshold: minRelevanceScore,
                                            requiredTags: requiredTags,
                                            top: limit,
                                            withVectors: withEmbeddings,
                                            cancellationToken: cancellationToken).ConfigureAwait(false);
        var result = results.Match(
                scoredPoints =>
                    {
                        var list = new List<(MemoryRecord, double)>();
                        foreach (var point in scoredPoints)
                        {
                            list.Add((ToMemoryRecord(point), point.Score));
                        }
                        return list;
                    },
                    errorResponse =>
                    {
                        _log.LogError("Error getting similar list: {error}", errorResponse.Error);
                        return default!;
                    }
                );
        foreach (var tuple in result)
        {
            yield return tuple;
        }
    }

    public MemoryRecord ToMemoryRecord(ScoredPoint point, bool withEmbedding = true)
    {
        var result = new MemoryRecord()
        {
            Id = point.PointId,
            Payload = (Dictionary<string, object>)point.Payload!,
        };

        if (withEmbedding)
        {
            result.Vector = point.Vector;
        }

        if (point.Tags is not null)
        {
            foreach (var keyValue in point.Tags!.Select(tag => tag.Split(Constants.ReservedEqualsSymbol, 2)))
            {
                var key = keyValue[0];
                var value = keyValue.Length == 1 ? null : keyValue[1];
                result.Tags.Add(key, value);
            }
        }
        return result;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<MemoryRecord> GetListAsync(string indexName, MemoryFilter? filter = null, int limit = 1, bool withEmbeddings = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        indexName = NormalizeIndexName(indexName);
        if (limit <= 0)
        {
            limit = int.MaxValue;
        }

        var requiredTags = filter != null && !filter.IsEmpty() ? filter.GetFilters().Select(x => $"{x.Key}{Constants.ReservedEqualsSymbol}{x.Value}") : new List<string>();
        var query = new SearchRequest()
            .WithPayload(true)
            .WithOffset(0)
            .WithVector(withEmbeddings)
            .HavingTags(requiredTags.ToArray())
            .Take(limit);

        var results = await qdrantClient!.Search(collectionName: indexName, query, cancellationToken: cancellationToken).ConfigureAwait(false);
        var result = results.Match(
           scoredPoints =>
               {
                   var list = new List<MemoryRecord>();
                   foreach (var point in scoredPoints)
                   {
                       list.Add(ToMemoryRecord(point));
                   }
                   return list;
               },
           errorResponse =>
               {
                   _log.LogError("Error getting similar list: {error}", errorResponse.Error);
                   return default!;
               }
       );
        foreach (var memoryRecord in result)
        {
            yield return memoryRecord;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string indexName, MemoryRecord record, CancellationToken cancellationToken = default)
    {
        indexName = NormalizeIndexName(indexName);

        var existingPoint = await qdrantClient!.GetVectorByIdAsync(indexName, record.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (existingPoint is null)
        {
            _log.LogTrace("No record with ID {0} found, nothing to delete", record.Id);
            return;
        }

        _log.LogTrace("Point ID {0} found, deleting...", existingPoint.PointId);
        await qdrantClient!.Delete(indexName, new List<string> { existingPoint.PointId }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private string NormalizeIndexName(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            indexName = Constants.DefaultIndex;
        }

        return indexName.Trim();
    }
}
