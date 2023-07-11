using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Collections;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using Microsoft.Extensions.Options;

using OneOf;

using Serilog;

using SerilogTimings;
using SerilogTimings.Extensions;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

/// <summary>
/// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection" />
/// </summary>
public class QdrantDb : IVectorDb
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly QdrantOptions options;


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

    public QdrantDb(IOptions<QdrantOptions> options, HttpClient httpClient, ILogger logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.options = options.Value;
        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
    private async Task<OneOf<TR, ErrorResponse>> VerifyResult<TR>(HttpResponseMessage response, string subUri, Operation op, CancellationToken cancellationToken)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
        {
            var result = await response.Content.ReadFromJsonAsync<QdrantHttpResponse<TR>>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return await HandleError(response, subUri, cancellationToken);
            }
            if (result.Status is not OperationStatus.Succeeded)
            {
                return await HandleError(response, subUri, cancellationToken);
            }
            op.Complete();
            return result!.Result;
        }
        return await HandleError(response, subUri, cancellationToken);
    }
    private OneOf<TR, ErrorResponse> VerifyResult<TR>(QdrantHttpResponse<TR>? response, Operation op)
    {
        if (response is null)
        {
            return new ErrorResponse($"Operation failed.");
        }

        if (response.Status is not OperationStatus.Succeeded)
        {
            return new ErrorResponse($"Operation failed with status {response.Status}.");
        }

        op.Complete();
        return response!.Result;
    }

    private ErrorResponse HandleError(Exception ex, string subUri)
    {
        logger.Error(ex, "Operation Failed for {uri}", subUri);
        return new ErrorResponse("Operation Failed for" + subUri + ex.Message);
    }

    private async Task<ErrorResponse> HandleError(HttpResponseMessage? response, string subUri, CancellationToken cancellationToken, Exception? ex = null)
    {
        if (ex is not null)
        {
            logger.Error(ex, "Operation Failed {uri}", subUri);
            return new ErrorResponse("Fatal Error Calling Qdrant.\n" + ex.Message);
        }
        if (response is not null && response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var result = await response.Content.ReadFromJsonAsync<HttpStatusResponse>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return new ErrorResponse($"Operation failed.");
            }

            if (result.Status is not null)
            {
                return new ErrorResponse($"Operation failed with status: {result.Status.Error}.");
            }
        }
        return new ErrorResponse("Fatal Error Calling Qdrant");
    }


    private async Task<OneOf<TR, ErrorResponse>> PostAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PostAsync {subUri}");
        HttpResponseMessage? response = null;
        try
        {
            PrepareClient();
            response = await httpClient.PostAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
            return await VerifyResult<TR>(response, subUri, op, cancellationToken);
        }
        catch (Exception ex)
        {
            return await HandleError(response, subUri, cancellationToken, ex);
        }
    }

    private async Task<OneOf<TR, ErrorResponse>> PutAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PutAsync {subUri}");
        HttpResponseMessage? response = null;
        try
        {
            PrepareClient();
            response = await httpClient.PutAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
            return await VerifyResult<TR>(response, subUri, op, cancellationToken);
        }
        catch (Exception ex)
        {
            return await HandleError(response, subUri, cancellationToken, ex);
        }
    }

    private async Task<OneOf<TR, ErrorResponse>> GetAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"GetAsync {subUri}");
        try
        {
            PrepareClient();
            var response = await httpClient.GetFromJsonAsync<QdrantHttpResponse<TR>>(subUri, cancellationToken)!;
            return VerifyResult(response, op);
        }
        catch (Exception ex)
        {
            return HandleError(ex, subUri);
        }
    }

    private async Task<OneOf<TR, ErrorResponse>> DeleteAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"DeleteAsync {subUri}");
        try
        {
            PrepareClient();
            var response = await httpClient.DeleteFromJsonAsync<QdrantHttpResponse<TR>>(subUri, cancellationToken);
            return VerifyResult(response, op);
        }
        catch (Exception ex)
        {
            return HandleError(ex, subUri);
        }
    }

    private void PrepareClient()
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
    }

    public VectorParams CreateParams(int? dimension = null, string? distance = null, bool? storeOnDisk = null)
    {
        if (string.IsNullOrEmpty(distance))
        {
            distance = options.DefaultDistance;
        }
        if (!storeOnDisk.HasValue)
        {
            storeOnDisk = options.DefaultStoreOnDisk;
        }
        if (!dimension.HasValue)
        {
            dimension = options.DefaultDimension;
        }
        var p = new VectorParams(size: dimension.Value, distance: distance, storeOnDisk.Value);
        return p;
    }

    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, VectorParams vectorsConfig, CancellationToken cancellationToken)
    {
        var payLoad = new CreateCollectionBody()
        {
            Vectors = vectorsConfig
        };
        var res = await PutAsync<CreateCollectionBody, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
        return res;
    }
    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CreateCollectionBody payLoad, CancellationToken cancellationToken)
    {
        var res = await PutAsync<CreateCollectionBody, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<bool, ErrorResponse>> CreateCollection(string collectionName, CollectCreationBodyWithMultipleNamedVectors payLoad, CancellationToken cancellationToken)
    {
        var res = await PutAsync<CollectCreationBodyWithMultipleNamedVectors, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
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
                foreach (var collectionName in collection)
                {
                    await DeleteCollection(collectionName, cancellationToken);
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

    public async Task<OneOf<bool, ErrorResponse>> Delete(string collectionName, BatchStruct batch, CancellationToken cancellationToken)
    {
        var payLoad = new BatchUpsertBody(batch);
        var result = await PostAsync<BatchUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors/delete?wait=true", payLoad, cancellationToken);
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

    public async Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchStruct batch, CancellationToken cancellationToken)
    {
        var payLoad = new BatchUpsertBody(batch);
        var result = await PostAsync<BatchUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    /// <inheritdoc />
    public async Task<OneOf<bool, ErrorResponse>> Update(string collectionName, BatchUpsertBody payLoad, CancellationToken cancellationToken)
    {
        var result = await PostAsync<BatchUpsertBody, UpdateResult>($"/collections/{collectionName}/points/vectors?wait=true", payLoad, cancellationToken);
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
    /// <param name="batch"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchStruct batch, CancellationToken cancellationToken)
    {
        var payLoad = new BatchUpsertBody(batch);
        var result = await PutAsync<BatchUpsertBody, UpdateResult>($"/collections/{collectionName}/points?wait=true", payLoad, cancellationToken);
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
    public async Task<OneOf<bool, ErrorResponse>> Upsert(string collectionName, BatchUpsertBody payLoad, CancellationToken cancellationToken)
    {
        var result = await PutAsync<BatchUpsertBody, UpdateResult>($"/collections/{collectionName}/points?wait=true", payLoad, cancellationToken);
        return result.Match<OneOf<bool, ErrorResponse>>(
            updateResult => updateResult.Status == UpdateStatus.ACKNOWLEDGED,
            error => error
        );
    }

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, double[] queryVector, CancellationToken cancellationToken, int limit = 10, int offset = 0)
    {
        var payLoad = new SearchBody()
        {
            Limit = limit,
            Offset = offset,
        };
        payLoad.SetVector(queryVector);
        var res = await PostAsync<SearchBody, ScoredPoint[]>($"/collections/{collectionName}/points/search", payLoad, cancellationToken);
        return res;
    }

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, float[] queryVector, CancellationToken cancellationToken, int limit = 10, int offset = 0)
    {
        var payLoad = new SearchBody()
        {
            Limit = limit,
            Offset = offset,
        };
        payLoad.SetVector(queryVector);
        var res = await PostAsync<SearchBody, ScoredPoint[]>($"/collections/{collectionName}/points/search", payLoad, cancellationToken);
        return res;
    }
    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, SearchBody query, CancellationToken cancellationToken)
    {
        var res = await PostAsync<SearchBody, ScoredPoint[]>($"/collections/{collectionName}/points/search", query, cancellationToken);
        return res;
    }
}
