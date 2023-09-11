using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using AI.Library.Utils;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;

using OneOf;

using Serilog;
using Serilog.Events;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

/// <summary>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html" >Qdrant</a>
/// </summary>
public class QdrantClient : QdrantClientBase, IQdrantClient
{
    private readonly ILogger logger;
    private readonly QdrantOptions options;
    private int? vectorSize = 0;
    private string activeCollectionName;

    public class HttpStatusResponse
    {
        [JsonPropertyName("status")]
        public HttpErrorResponse? Status { get; init; }
        [JsonPropertyName("time")]
        public double Time { get; init; }
    }
    public class HttpErrorResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; init; }
    }
    public void SetCollectionName(string collectionName)
    {
        this.activeCollectionName = collectionName;
    }
    public void SetVectorSize(int dimension)
    {
        this.vectorSize = dimension;
    }

    public void UseParams(VectorParams @params)
    {
        this.vectorSize = @params.Size;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    /// <param name="httpClient"></param>
    /// <param name="logger"></param>
    public QdrantClient(IOptions<QdrantOptions> options, HttpClient httpClient, ILogger logger) : base(httpClient, options.Value, logger)
    {
        this.logger = logger;
        this.options = options.Value;
    }

    /// <summary>
    /// Create a Collection
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="vectorsConfig"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorsConfig, CancellationToken cancellationToken)
    {
        var payLoad = new CreateCollectionWithVectorRequest()
        {
            Vectors = vectorsConfig
        };
        this.vectorSize = vectorsConfig.Size;
        var res = await PutAsync<CreateCollectionWithVectorRequest, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CreateCollectionWithVectorRequest payLoad, CancellationToken cancellationToken)
    {
        var res = await PutAsync<CreateCollectionWithVectorRequest, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CancellationToken cancellationToken)
    {
        var res = await PutAsync<bool>($"/collections/{collectionName}´?timeout=10", cancellationToken);
        return res;
    }

    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CreateCollectionWithMultipleNamedVectorsRequest payLoad, CancellationToken cancellationToken)
    {
        var res = await PutAsync<CreateCollectionWithMultipleNamedVectorsRequest, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<CollectionList, ErrorResponse>> GetCollections(CancellationToken cancellationToken)
    {
        return await GetAsync<CollectionList>("/collections", cancellationToken);
    }

    public async Task<OneOf<IList<string>, ErrorResponse>> GetCollectionNames(CancellationToken cancellationToken)
    {
        var result = await GetCollections(cancellationToken);
        return result.Match<OneOf<IList<string>, ErrorResponse>>(
            collections => collections.Collections.Select(x => x.Name).ToList(),
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> DoesCollectionExist(string collectionName, CancellationToken cancellationToken)
    {
        var result = await GetCollections(cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            collections => collections.Collections.Exists(x => x.Name == collectionName),
            error => error
        );
    }

    public async Task<OneOf<CollectionInfo, ErrorResponse>> GetCollection(string collectionName, CancellationToken cancellationToken)
    {
        return await GetAsync<CollectionInfo>($"/collections/{collectionName}", cancellationToken);
    }

    public async Task<OneOf<bool, ErrorResponse>> RemoveAllCollections(CancellationToken cancellationToken)
    {
        var allCollection = await GetCollectionNames(cancellationToken);
        return await allCollection.Match<Task<OneOf<bool, ErrorResponse>>>(

            async collection =>
            {
                foreach (var colName in collection)
                {
                    await DeleteCollection(colName, cancellationToken);
                }
                return true;
            },
            error => Task.FromResult<OneOf<bool, ErrorResponse>>(new ErrorResponse($"Failed to Remove all collections: {error.Error}"))
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> RemoveCollection(string collectionName, CancellationToken cancellationToken)
    {
        return await DeleteCollection(collectionName, cancellationToken);
    }

    public async Task<OneOf<bool, ErrorResponse>> DeleteCollection(string collectionName, CancellationToken cancellationToken)
    {
        var result = await DeleteAsync<bool>($"/collections/{collectionName}", cancellationToken);
        return result;
    }

    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PostAsync<PointsUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors/delete?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }
    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsWithNamedVectorsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PostAsync<PointsWithNamedVectorsUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors/delete?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, BatchRequestStruct batchRequest, CancellationToken cancellationToken)
    {
        var payLoad = new BatchUpsertRequest(batchRequest);
        var result = await PostAsync<BatchUpsertRequest, UpdateResult>($"/collections/{collectionName}/points/vectors/delete?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> DeletePayloadKeys(string collectionName, DeleteFilter filter, CancellationToken cancellationToken)
    {
        var result = await PostAsync<DeleteFilter, UpdateResult>($"/collections/{collectionName}/points/payload/delete?wait=true", filter, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IEnumerable<string> pointIds, CancellationToken cancellationToken)
    {
        var filter = new DeletePointsFilter()
        {
            Ids = pointIds.ToList()
        };
        var result = await PostAsync<DeletePointsFilter, UpdateResult>($"/collections/{collectionName}/points/delete?wait=true", filter, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }


    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IEnumerable<PointStruct> points, CancellationToken cancellationToken)
    {
        var filter = new DeletePointsFilter()
        {
            Ids = points!.Select(p => p.Id).ToList()
        };

        var result = await PostAsync<DeletePointsFilter, UpdateResult>($"/collections/{collectionName}/points/delete?wait=true", filter, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, IEnumerable<PointStructWithNamedVector> points, CancellationToken cancellationToken)
    {
        var filter = new DeletePointsFilter()
        {
            Ids = points!.Select(p => p.Id).ToList()
        };

        var result = await PostAsync<DeletePointsFilter, UpdateResult>($"/collections/{collectionName}/points/delete?wait=true", filter, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, string payloadId, CancellationToken cancellationToken)
    {
        var filter = new DeletePointsFilter(payloadId);
        var result = await PostAsync<DeletePointsFilter, UpdateResult>($"/collections/{collectionName}/points/delete?wait=true", filter, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> Update(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PostAsync<PointsUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }
    public async Task<OneOf<bool, ErrorResponse>> Update(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsWithNamedVectorsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PostAsync<PointsWithNamedVectorsUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchRequestStruct batchRequest, CancellationToken cancellationToken)
    {
        var payLoad = new BatchUpsertRequest(batchRequest);
        var result = await PostAsync<BatchUpsertRequest, UpdateResult>($"/collections/{collectionName}/points/vectors?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    /// <inheritdoc />
    public async Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchUpsertRequest payLoad, CancellationToken cancellationToken)
    {
        var result = await PostAsync<BatchUpsertRequest, UpdateResult>($"/collections/{collectionName}/points/vectors?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/payload/#set-payload">Set Payload</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="points"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> SetPayload(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PostAsync<PointsUpsertBody, UpdateResult>($"/collections/{collectionName}/points/payload?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }
    /// <summary>
    ///   /// <a href="https://qdrant.tech/documentation/concepts/payload/#delete-payload">Delete Payload</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="points"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> DeletePayload(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PostAsync<PointsUpsertBody, UpdateResult>($"/collections/{collectionName}/points/payload/delete?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }


    /// <summary>
    /// a href= "https://qdrant.tech/documentation/concepts/points/#upload-points" />
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="points"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStruct> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PutAsync<PointsUpsertBody, UpdateResult>($"/collections/{collectionName}/points?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    /// <summary>
    /// a href= "https://qdrant.tech/documentation/concepts/points/#upload-points" />
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="points"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, IList<PointStructWithNamedVector> points, CancellationToken cancellationToken)
    {
        var payLoad = new PointsWithNamedVectorsUpsertBody()
        {
            Points = points.ToList()
        };
        var result = await PutAsync<PointsWithNamedVectorsUpsertBody, UpdateResult>($"/collections/{collectionName}/points?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }
    /// <summary>
    /// a href= "https://qdrant.tech/documentation/concepts/points/#upload-points" />
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="batchRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchRequestStruct batchRequest, CancellationToken cancellationToken)
    {
        var payLoad = new BatchUpsertRequest(batchRequest);
        var result = await PutAsync<BatchUpsertRequest, UpdateResult>($"/collections/{collectionName}/points?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }
    /// <summary>
    /// a href= "https://qdrant.tech/documentation/concepts/points/#upload-points" />
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="payLoad"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchUpsertRequest payLoad, CancellationToken cancellationToken)
    {
        //var s = JsonSerializer.Serialize(payLoad, DefaultJsonSerializerOptions.DefaultOptions);
        var result = await PutAsync<BatchUpsertRequest, UpdateResult>($"/collections/{collectionName}/points?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, double[] queryVector, int limit = 10, int offset = 0, CancellationToken cancellationToken = default)
    {
        var payLoad = new SearchRequest()
        {
            Limit = limit,
            Offset = offset,
        };
        payLoad.SimilarToVector(queryVector);
        var res = await PostAsync<SearchRequest, ScoredPoint[]>($"/collections/{collectionName}/points/search", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, float[] queryVector, int limit = 10, int offset = 0, CancellationToken cancellationToken = default)
    {
        var payLoad = new SearchRequest()
        {
            Limit = limit,
            Offset = offset,
        };
        payLoad.SimilarToVector(queryVector);
        var res = await PostAsync<SearchRequest, ScoredPoint[]>($"/collections/{collectionName}/points/search", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> FindNearestInCollection(string collectionName, IEnumerable<float> target, double threshold,
            int top = 1, bool withVectors = false, IEnumerable<string>? requiredTags = null, CancellationToken cancellationToken = default)
    {
        var search = new SearchRequest()
            .SimilarToVector(target)
            .HavingTags(requiredTags)
            .WithScoreThreshold(threshold)
            .IncludePayLoad()
            .IncludeVectorData(withVectors)
            .Take(top)
           ;

        var result = await Search(collectionName, search, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<QdrantVectorRecord?> GetVectorByIdAsync(string collectionName, string pointId, bool withVector = false, CancellationToken cancellationToken = default)
    {
        this.logger.Debug("Searching vector by point ID");

        var search = new SearchRequest()
                .SimilarToVector(new float[this.vectorSize!.Value])
                .HavingExternalId(pointId)
                .IncludePayLoad()
                .TakeFirst()
                .IncludeVectorData(withVector)
            ;

        var searchResult = await Search(collectionName, search, cancellationToken: cancellationToken);

        var record = searchResult.Match<QdrantVectorRecord?>(
            points =>
            {
                if (points.Length > 0)
                {
                    var point = points[0];
                    var record = new QdrantVectorRecord(
                        pointId: point.Id,
                        embedding: new ReadOnlyMemory<float>(point.Vector.ToArray()),
                        payload: (Dictionary<string, object>)point.Payload,
                        tags: null);
                    return record;
                }
                else
                {
                    return null;
                }
            },
           error => null
        );
        this.logger.Debug("Vector found}");
        return record;
    }


    public async IAsyncEnumerable<QdrantVectorRecord> GetVectorsByIdAsync(string collectionName, IEnumerable<string> pointIds, bool withVectors = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        this.logger.Debug("Searching vectors by point ID's");
        var search = new SearchRequest()
                .SimilarToVector(new float[this.vectorSize!.Value])
                .HavingExternalId(pointIds)
                .IncludePayLoad()
                .TakeFirst()
                .IncludeVectorData(withVectors)
            ;

        var searchResult = await Search(collectionName, search, cancellationToken: cancellationToken);
        foreach (var r in searchResult.Match(
                     ProcessRecords,
                     error =>
                     {
                         logger.Warning($"Vectors from {collectionName} not found");
                         return Enumerable.Empty<QdrantVectorRecord?>();
                     }))
        {
            yield return r!;
        }
    }

    private IEnumerable<QdrantVectorRecord> ProcessRecords(ScoredPoint[]? records)
    {
        if (records is null)
        {
            yield break;
        }
        foreach (var record in records)
        {
            var rg = new QdrantVectorRecord(pointId: record.Id, embedding: record.Vector, (Dictionary<string, object>)record.Payload!, tags: null);
            yield return rg;
        }
    }


    /// <summary>
    /// Similar to GetVectorsByIdAsync
    /// </summary>
    /// <param name="colName"></param>
    /// <param name="id"></param>
    /// <param name="withVectors"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<ScoredPoint[], ErrorResponse>> SearchByPayloadId(string colName, string id, bool withVectors = false, CancellationToken cancellationToken = default)
    {
        var search = new SearchRequest()
                .SimilarToVector(new float[this.vectorSize!.Value])
                .HavingExternalId(id)
                .IncludePayLoad()
                .TakeFirst()
                .IncludeVectorData(withVectors)
                .WithScoreThreshold(-1)
            ;
        var result = await Search(colName, search, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> SearchByPayloadIds(string collectionName, IEnumerable<string> pointIds, bool withVectors = false, int limit = 10, int offset = 0, CancellationToken cancellationToken = default)
    {
        var ids = pointIds.ToList();
        var search = new SearchRequest()
            .SimilarToVector(new float[this.vectorSize!.Value])
            .HavingExternalId(ids)
            .IncludePayLoad()
            .Take(ids.Count)
            .IncludeVectorData(withVectors)
            .WithScoreThreshold(-1)
            ;

        var result = await Search(collectionName, search, cancellationToken: cancellationToken);
        return result;
    }


    public async Task<OneOf<ScoredPoint, NullResult, ErrorResponse>> SearchSingleByPayloadId(string collectionName, string id, bool withVectors = false, CancellationToken cancellationToken = default)
    {
        var search = new SearchRequest()
                .SimilarToVector(new float[this.vectorSize!.Value])
                .HavingExternalId(id)
                .IncludePayLoad()
                .TakeFirst()
                .IncludeVectorData(withVectors)
                .WithScoreThreshold(-1)
            ;
        var result = await Search(collectionName, search, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.Match<OneOf<ScoredPoint, NullResult, ErrorResponse>>(
            points => points.Length > 0 ? points[0] : new NullResult(),
            error => error
        );
    }


    public async Task<OneOf<ScoredPoint[], ErrorResponse>> SearchByPointIds(string collectionName, IEnumerable<string> pointIds, bool withVectors = false, CancellationToken cancellationToken = default)
    {
        var search = new SearchRequest()
                //.SimilarToVector(new float[this.vectorSize!.Value])
                .WithPointIDs(pointIds)
                .UseWithPayload(true)
                .SetWithVector(withVectors)
                .WithScoreThreshold(0)
            ;
        var s = JsonSerializer.Serialize(search, DefaultJsonSerializerOptions.DefaultOptions);
        logger.Debug(s);
        var result = await SearchUsingPointId(collectionName, search, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<OneOf<ScoredPoint, NullResult, ErrorResponse>> SearchSingleByPointId(string collectionName, string pointId, bool withVectors = false, CancellationToken cancellationToken = default)
    {
        var search = new SearchRequest()
                //.SimilarToVector(new float[this.vectorSize!.Value])
                .WithPointId(pointId)
                .UseWithPayload(true)
                .TakeFirst()
                .SetWithVector(withVectors)
                .WithScoreThreshold(0)
                ;

        var result = await SearchUsingPointId(collectionName, search, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.Match<OneOf<ScoredPoint, NullResult, ErrorResponse>>(
            points => points.Length > 0 ? points[0] : new NullResult(),
            error => error
        );
    }


    public async Task<OneOf<ScoredPoint[], ErrorResponse>> SearchUsingPointId(string collectionName, SearchRequest query, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogEventLevel.Debug))
        {
            var s = JsonSerializer.Serialize(query, DefaultJsonSerializerOptions.DefaultOptions);
            logger.Debug(s);
        }
        var res = await PostAsync<SearchRequest, ScoredPoint[]>($"/collections/{collectionName}/points", query, cancellationToken);
        return res;
    }



    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, SearchRequest query, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogEventLevel.Debug))
        {
            var s = JsonSerializer.Serialize(query, DefaultJsonSerializerOptions.DefaultOptions);
            logger.Debug(s);
        }
        var res = await PostAsync<SearchRequest, ScoredPoint[]>($"/collections/{collectionName}/points/search", query, cancellationToken);
        return res;
    }

}



