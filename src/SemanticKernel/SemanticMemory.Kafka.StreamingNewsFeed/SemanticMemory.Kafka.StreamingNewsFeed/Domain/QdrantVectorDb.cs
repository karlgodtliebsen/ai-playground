namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

//public class QdrantVectorDb : ISemanticMemoryVectorDb
//{

//    private IQdrantClient? _qdrantClient;
//    private readonly IQdrantFactory qdrantFactory;
//    private readonly ILogger<QdrantVectorDb> _log;


//    public QdrantVectorDb(IQdrantFactory qdrantFactory, ILogger<QdrantVectorDb>? log = null)
//    {
//        this.qdrantFactory = qdrantFactory;
//        _log = log ?? DefaultLogger<QdrantVectorDb>.Instance;
//    }

//    /// <inheritdoc />
//    public async Task CreateIndexAsync(string indexName, int vectorSize, CancellationToken cancellationToken = default)
//    {
//        var vectorParams = qdrantFactory.CreateParams(vectorSize, Distance.DOT, true);
//        _qdrantClient = await qdrantFactory.Create(indexName, vectorParams, cancellationToken: cancellationToken);
//        await _qdrantClient.CreateCollection(indexName, vectorParams, cancellationToken);
//    }

//    /// <inheritdoc />
//    public async Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
//    {
//        indexName = NormalizeIndexName(indexName);
//        if (string.Equals(indexName, Constants.DefaultIndex, StringComparison.OrdinalIgnoreCase))
//        {
//            _log.LogWarning("The default index cannot be deleted");
//            return;
//        }
//        await _qdrantClient!.RemoveCollection(indexName, cancellationToken);
//    }

//    /// <inheritdoc />
//    public async Task<string> UpsertAsync(string indexName, MemoryRecord record, CancellationToken cancellationToken = default)
//    {
//        indexName = NormalizeIndexName(indexName);

//        //QdrantPoint<DefaultQdrantPayload> qdrantPoint;

//        if (string.IsNullOrEmpty(record.Id))
//        {
//            record.Id = Guid.NewGuid().ToString("N");

//            var vectorData = await ConvertFromMemoryRecordAsync(collectionName, record, cancellationToken).ConfigureAwait(false);
//            if (vectorData == null)
//            {
//                throw new SKException("Failed to convert memory record to Qdrant vector record");
//            }

//            qdrantPoint = QdrantPoint<DefaultQdrantPayload>.FromMemoryRecord(record);

//            qdrantPoint.Id = Guid.NewGuid();

//            _log.LogTrace("Generate new Qdrant point ID {0} and record ID {1}", qdrantPoint.Id, record.Id);
//        }
//        else
//        {
//            qdrantPoint = QdrantPoint<DefaultQdrantPayload>.FromMemoryRecord(record);

//            var existingPoint =
//                await _qdrantClient.GetVectorsByIdAsync(indexName, record.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
//            if (existingPoint == null)
//            {
//                qdrantPoint.Id = Guid.NewGuid();
//                _log.LogTrace("No record with ID {0} found, generated a new point ID {1}", record.Id, qdrantPoint.Id);
//            }
//            else
//            {
//                qdrantPoint.Id = existingPoint.Id;
//                _log.LogTrace("Point ID {0} found, updating...", qdrantPoint.Id);
//            }
//        }

//        await _qdrantClient.Upsert(indexName, new[] { qdrantPoint }, cancellationToken).ConfigureAwait(false);

//        return record.Id;
//    }

//    /// <inheritdoc />
//    public async IAsyncEnumerable<(MemoryRecord, double)> GetSimilarListAsync(
//        string indexName,
//        Embedding embedding,
//        int limit,
//        double minRelevanceScore = 0,
//        MemoryFilter? filter = null,
//        bool withEmbeddings = false,
//        [EnumeratorCancellation] CancellationToken cancellationToken = default)
//    {
//        indexName = NormalizeIndexName(indexName);
//        if (limit <= 0) { limit = int.MaxValue; }

//        var requiredTags = filter != null && !filter.IsEmpty()
//            ? filter.GetFilters().Select(x => $"{x.Key}{Constants.ReservedEqualsSymbol}{x.Value}")
//            : new List<string>();

//        var results = await _qdrantClient.FindNearestInCollection(
//            collectionName: indexName,
//            target: embedding,
//            // minSimilarityScore: minRelevanceScore,
//            requiredTags: requiredTags,
//            // limit: limit,
//            withVectors: withEmbeddings,
//            cancellationToken: cancellationToken).ConfigureAwait(false);

//        foreach (var point in results)
//        {
//            yield return (point.Item1.ToMemoryRecord(), point.Item2);
//        }
//    }

//    /// <inheritdoc />
//    public async IAsyncEnumerable<MemoryRecord> GetListAsync(
//        string indexName,
//        MemoryFilter? filter = null,
//        int limit = 1,
//        bool withEmbeddings = false,
//        [EnumeratorCancellation] CancellationToken cancellationToken = default)
//    {
//        indexName = NormalizeIndexName(indexName);
//        if (limit <= 0) { limit = int.MaxValue; }

//        var requiredTags = filter != null && !filter.IsEmpty() ? filter.GetFilters().Select(x => $"{x.Key}{Constants.ReservedEqualsSymbol}{x.Value}") : new List<string>();

//        var results = await _qdrantClient.GetVectorsByIdAsync(
//            collectionName: indexName,
//            //requiredTags: requiredTags,
//            //offset: 0,
//            //limit: limit,
//            withVectors: withEmbeddings,
//            cancellationToken: cancellationToken).ConfigureAwait(false);

//        foreach (var point in results)
//        {
//            yield return point.ToMemoryRecord();
//        }
//    }

//    /// <inheritdoc />
//    public async Task DeleteAsync(
//        string indexName,
//        MemoryRecord record,
//        CancellationToken cancellationToken = default)
//    {
//        indexName = NormalizeIndexName(indexName);

//        var existingPoint = await _qdrantClient.GetVectorByPayloadIdAsync(indexName, record.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

//        if (existingPoint == null)
//        {
//            _log.LogTrace("No record with ID {0} found, nothing to delete", record.Id);
//            return;
//        }

//        _log.LogTrace("Point ID {0} found, deleting...", existingPoint.Id);
//        await _qdrantClient.DeleteVectorsAsync(indexName, new List<Guid> { existingPoint.Id }, cancellationToken).ConfigureAwait(false);
//    }

//    private string NormalizeIndexName(string indexName)
//    {
//        if (string.IsNullOrWhiteSpace(indexName))
//        {
//            indexName = Constants.DefaultIndex;
//        }

//        return indexName.Trim();
//    }
//}
