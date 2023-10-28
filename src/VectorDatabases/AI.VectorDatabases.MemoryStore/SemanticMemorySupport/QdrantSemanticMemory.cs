namespace AI.VectorDatabases.MemoryStore.SemanticMemorySupport;

//public class QdrantSemanticMemory : ISemanticMemoryVectorDb
//{
//    private readonly QdrantClient<DefaultQdrantPayload> _qdrantClient;
//    private readonly ILogger<QdrantSemanticMemory> _log;

//    public QdrantSemanticMemory(QdrantConfig config, ILogger<QdrantSemanticMemory>? log = null)
//    {
//        this._log = log ?? DefaultLogger<QdrantSemanticMemory>.Instance;
//        this._qdrantClient = new QdrantClient<DefaultQdrantPayload>(config.Endpoint, config.APIKey);
//    }

//    public Task CreateIndexAsync(
//      string indexName,
//      int vectorSize,
//      CancellationToken cancellationToken = default(CancellationToken))
//    {
//        return this._qdrantClient.CreateCollectionAsync(indexName, vectorSize, cancellationToken);
//    }

//    public Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default(CancellationToken))
//    {
//        indexName = this.NormalizeIndexName(indexName);
//        if (!string.Equals(indexName, "default", StringComparison.OrdinalIgnoreCase))
//            return this._qdrantClient.DeleteCollectionAsync(indexName, cancellationToken);
//        this._log.LogWarning("The default index cannot be deleted");
//        return Task.CompletedTask;
//    }

//    public async Task<string> UpsertAsync(
//      string indexName,
//      MemoryRecord record,
//      CancellationToken cancellationToken = default(CancellationToken))
//    {
//        indexName = this.NormalizeIndexName(indexName);
//        QdrantPoint<DefaultQdrantPayload> qdrantPoint;
//        if (string.IsNullOrEmpty(record.Id))
//        {
//            record.Id = Guid.NewGuid().ToString("N");
//            qdrantPoint = QdrantPoint<DefaultQdrantPayload>.FromMemoryRecord(record);
//            qdrantPoint.Id = Guid.NewGuid();
//            this._log.LogTrace("Generate new Qdrant point ID {0} and record ID {1}", (object)qdrantPoint.Id, (object)record.Id);
//        }
//        else
//        {
//            qdrantPoint = QdrantPoint<DefaultQdrantPayload>.FromMemoryRecord(record);
//            QdrantPoint<DefaultQdrantPayload> qdrantPoint1 = await this._qdrantClient.GetVectorByPayloadIdAsync(indexName, record.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
//            if (qdrantPoint1 == null)
//            {
//                qdrantPoint.Id = Guid.NewGuid();
//                this._log.LogTrace("No record with ID {0} found, generated a new point ID {1}", (object)record.Id, (object)qdrantPoint.Id);
//            }
//            else
//            {
//                qdrantPoint.Id = qdrantPoint1.Id;
//                this._log.LogTrace("Point ID {0} found, updating...", (object)qdrantPoint.Id);
//            }
//        }
//        await this._qdrantClient.UpsertVectorsAsync(indexName, (IEnumerable<QdrantPoint<DefaultQdrantPayload>>)new QdrantPoint<DefaultQdrantPayload>[1]
//        {
//        qdrantPoint
//        }, cancellationToken).ConfigureAwait(false);
//        string id = record.Id;
//        qdrantPoint = (QdrantPoint<DefaultQdrantPayload>)null;
//        return id;
//    }

//    public async IAsyncEnumerable<(MemoryRecord, double)> GetSimilarListAsync(
//      string indexName,
//      Embedding embedding,
//      int limit,
//      double minRelevanceScore = 0.0,
//      ICollection<MemoryFilter>? filters = null,
//      bool withEmbeddings = false,
//      [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
//    {
//        indexName = this.NormalizeIndexName(indexName);
//        if (limit <= 0)
//            limit = int.MaxValue;
//        List<IEnumerable<string>> stringsList = new List<IEnumerable<string>>();
//        if (filters != null)
//            stringsList.AddRange(filters.Select<MemoryFilter, IEnumerable<string>>((Func<MemoryFilter, IEnumerable<string>>)(filter => filter.GetFilters().Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x =>
//            {
//                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
//                interpolatedStringHandler.AppendFormatted(x.Key);
//                interpolatedStringHandler.AppendFormatted<char>(':');
//                interpolatedStringHandler.AppendFormatted(x.Value);
//                return interpolatedStringHandler.ToStringAndClear();
//            })))));
//        QdrantClient<DefaultQdrantPayload> qdrantClient = this._qdrantClient;
//        string collectionName = indexName;
//        Embedding target = embedding;
//        double minSimilarityScore = minRelevanceScore;
//        IEnumerable<IEnumerable<string>> strings = (IEnumerable<IEnumerable<string>>)stringsList;
//        int limit1 = limit;
//        int num = withEmbeddings ? 1 : 0;
//        IEnumerable<IEnumerable<string>> requiredTags = strings;
//        CancellationToken cancellationToken1 = cancellationToken;
//        foreach ((QdrantPoint<DefaultQdrantPayload>, double) valueTuple in await qdrantClient.GetSimilarListAsync(collectionName, target, minSimilarityScore, limit1, num != 0, requiredTags, cancellationToken1).ConfigureAwait(false))
//            yield return (valueTuple.Item1.ToMemoryRecord(), valueTuple.Item2);
//        // ISSUE: reference to a compiler-generated field
//        this.\u003C\u003E2__current = ();
//    }

//    public async IAsyncEnumerable<MemoryRecord> GetListAsync(
//      string indexName,
//      ICollection<MemoryFilter>? filters = null,
//      int limit = 1,
//      bool withEmbeddings = false,
//      [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
//    {
//        indexName = this.NormalizeIndexName(indexName);
//        if (limit <= 0)
//            limit = int.MaxValue;
//        List<IEnumerable<string>> requiredTags = new List<IEnumerable<string>>();
//        if (filters != null)
//            requiredTags.AddRange(filters.Select<MemoryFilter, IEnumerable<string>>((Func<MemoryFilter, IEnumerable<string>>)(filter => filter.GetFilters().Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x =>
//            {
//                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
//                interpolatedStringHandler.AppendFormatted(x.Key);
//                interpolatedStringHandler.AppendFormatted<char>(':');
//                interpolatedStringHandler.AppendFormatted(x.Value);
//                return interpolatedStringHandler.ToStringAndClear();
//            })))));
//        foreach (QdrantPoint<DefaultQdrantPayload> qdrantPoint in await this._qdrantClient.GetListAsync(indexName, (IEnumerable<IEnumerable<string>>)requiredTags, limit: limit, withVectors: withEmbeddings, cancellationToken: cancellationToken).ConfigureAwait(false))
//            yield return qdrantPoint.ToMemoryRecord();
//        // ISSUE: reference to a compiler-generated field
//        this.\u003C\u003E2__current = (MemoryRecord)null;
//    }

//    public async Task DeleteAsync(
//      string indexName,
//      MemoryRecord record,
//      CancellationToken cancellationToken = default(CancellationToken))
//    {
//        indexName = this.NormalizeIndexName(indexName);
//        QdrantPoint<DefaultQdrantPayload> qdrantPoint = await this._qdrantClient.GetVectorByPayloadIdAsync(indexName, record.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
//        if (qdrantPoint == null)
//        {
//            this._log.LogTrace("No record with ID {0} found, nothing to delete", (object)record.Id);
//        }
//        else
//        {
//            this._log.LogTrace("Point ID {0} found, deleting...", (object)qdrantPoint.Id);
//            QdrantClient<DefaultQdrantPayload> qdrantClient = this._qdrantClient;
//            string collectionName = indexName;
//            List<Guid> vectorIds = new List<Guid>();
//            vectorIds.Add(qdrantPoint.Id);
//            CancellationToken cancellationToken1 = cancellationToken;
//            await qdrantClient.DeleteVectorsAsync(collectionName, (IList<Guid>)vectorIds, cancellationToken1).ConfigureAwait(false);
//        }
//    }

//    private string NormalizeIndexName(string indexName)
//    {
//        if (string.IsNullOrWhiteSpace(indexName))
//            indexName = "default";
//        return indexName.Trim();
//    }
//}
