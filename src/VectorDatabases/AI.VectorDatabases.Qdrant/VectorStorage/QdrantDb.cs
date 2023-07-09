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

    private async Task<OneOf<TR, ErrorResponse>> PostAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PostAsync {subUri}");
        try
        {
            PrepareClient();
            var response = await httpClient.PostAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Models.QdrantHttpResponse<TR>>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return new ErrorResponse($"PostAsync failed.");
            }
            if (result.Status is not OperationStatus.Succeeded)
            {
                return new ErrorResponse($"PostAsync failed with status {result.Status}.");
            }
            op.Complete();
            return result!.Result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "PostAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }
    private async Task<OneOf<TR, ErrorResponse>> PutAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PutAsync {subUri}");
        try
        {
            PrepareClient();
            var response = await httpClient.PutAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Models.QdrantHttpResponse<TR>>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return new ErrorResponse($"PutAsync failed.");
            }
            if (result.Status is not OperationStatus.Succeeded)
            {
                return new ErrorResponse($"PutAsync failed with status {result.Status}.");
            }
            op.Complete();
            return result!.Result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "PutAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    private async Task<OneOf<TR, ErrorResponse>> GetAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"GetAsync {subUri}");
        try
        {
            PrepareClient();
            var result = await httpClient.GetFromJsonAsync<Models.QdrantHttpResponse<TR>>(subUri, cancellationToken)!;
            if (result is null)
            {
                return new ErrorResponse($"GetAsync failed.");
            }
            if (result.Status is not OperationStatus.Succeeded)
            {
                return new ErrorResponse($"GetAsync failed with status {result.Status}.");
            }
            op.Complete();
            return result!.Result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }
    private async Task<OneOf<TR, ErrorResponse>> DeleteAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"DeleteAsync {subUri}");
        try
        {
            PrepareClient();
            var result = await httpClient.DeleteFromJsonAsync<Models.QdrantHttpResponse<TR>>(subUri, cancellationToken);
            if (result is null)
            {
                return new ErrorResponse($"DeleteAsync failed.");
            }
            if (result.Status is not OperationStatus.Succeeded)
            {
                return new ErrorResponse($"DeleteAsync failed with status {result.Status}.");
            }
            op.Complete();
            return result!.Result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "DeleteAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
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
        var payLoad = new CollectCreationBody()
        {
            Vectors = vectorsConfig
        };
        var res = await PutAsync<CollectCreationBody, bool>($"/collections/{collectionName}", payLoad, cancellationToken);
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

    public async Task<OneOf<ScoredPoint[], ErrorResponse>> Search(string collectionName, float[] queryVector, CancellationToken cancellationToken,
        int limit = 10, int offset = 0)
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
