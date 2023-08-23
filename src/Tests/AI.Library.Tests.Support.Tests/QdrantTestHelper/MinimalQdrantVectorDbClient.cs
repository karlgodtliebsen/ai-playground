using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

internal class MinimalQdrantVectorDbClient : IQdrantVectorDbClient
{
    private static readonly HttpClient httpClient = new HttpClient();

    private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public MinimalQdrantVectorDbClient(string url)
    {
        httpClient.BaseAddress = new Uri(url);
    }

    /// <summary>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/create_collection">Documentation</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="vectorParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> CreateCollection(string collectionName, VectorParams vectorParams, CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Clear();
        var payLoad = new CreateCollectionWithVectorRequest()
        {
            Vectors = vectorParams
        };
        var response = await httpClient.PutAsJsonAsync($"/collections/{collectionName}", payLoad, serializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<QdrantHttpResponse<bool>>(cancellationToken: cancellationToken);
        return result!.Result;
    }

    /// <summary>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html#tag/collections/operation/get_collection">Documentation</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CollectionInfo> GetCollection(string collectionName, CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Clear();
        var response = await httpClient.GetFromJsonAsync<QdrantHttpResponse<CollectionInfo>>($"/collections/{collectionName}", cancellationToken)!;
        return response!.Result;
    }

    internal static class Distances
    {
        public const string DOT = "Dot";
    }

    /// <summary>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html">Documentation</a>
    /// </summary>
    internal class VectorParams
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("distance")]
        public string Distance { get; set; }


        /// <summary>
        /// Constructor for VectorParams
        /// </summary>
        /// <param name="size"></param>
        /// <param name="distance"></param>
        /// <param name="onDisk"></param>
        public VectorParams(int size, string distance)
        {
            Size = size;
            Distance = distance;
        }
    }
    /// <summary>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html">Documentation</a>
    /// </summary>
    private class CreateCollectionWithVectorRequest
    {
        /// <summary>
        /// Vectors collection name
        /// <a href="https://qdrant.tech/documentation/reference/collections.html#collection-name">Collection name</a>
        /// </summary>
        [JsonPropertyName("vectors")]
        public VectorParams Vectors { get; init; } = default!;
    }

    /// <summary>
    /// <a href="https://qdrant.github.io/qdrant/redoc/index.html">Documentation</a>
    /// </summary>
    internal class CollectionInfo
    {
        [JsonPropertyName("status")] public string Status { get; set; }

    }

    internal class QdrantHttpResponse<T>
    {
        [JsonPropertyName("time")] public float Time { get; set; }

        [JsonPropertyName("status")] public string Status { get; set; }

        [JsonPropertyName("result")] public T Result { get; set; } = default!;
    }
}

