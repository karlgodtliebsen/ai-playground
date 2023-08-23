using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

internal class QdrantVectorDbClient : IQdrantVectorDbClient
{
    private static readonly HttpClient httpClient = new HttpClient();

    private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public QdrantVectorDbClient(string url)
    {
        httpClient.BaseAddress = new Uri(url);
    }

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
        /// 
        /// </summary>
        [JsonPropertyName("on_disk")]
        public bool OnDisk { get; set; }

        /// <summary>
        /// Constructor for VectorParams
        /// </summary>
        /// <param name="size"></param>
        /// <param name="distance"></param>
        /// <param name="onDisk"></param>
        public VectorParams(int size, string distance, bool onDisk = false)
        {
            Size = size;
            Distance = distance;
            OnDisk = onDisk;
        }
    }

    private class CreateCollectionWithVectorRequest
    {
        /// <summary>
        /// Vectors collection name
        /// <a href="https://qdrant.tech/documentation/reference/collections.html#collection-name">Collection name</a>
        /// </summary>
        [JsonPropertyName("vectors")]
        public VectorParams Vectors { get; init; } = default!;
    }

    internal class CollectionInfo
    {
        [JsonPropertyName("status")] public string Status { get; set; }

        [JsonPropertyName("optimizer_status")] public string OptimizerStatus { get; set; }

        [JsonPropertyName("vectors_count")] public int VectorsCount { get; set; }

        [JsonPropertyName("indexed_vectors_count")] public int IndexedVectorsCount { get; set; }

        [JsonPropertyName("points_count")] public int PointsCount { get; set; }

        [JsonPropertyName("segments_count")] public int SegmentsCount { get; set; }

    }

    internal class QdrantHttpResponse<T>
    {
        [JsonPropertyName("time")] public float Time { get; set; }

        [JsonPropertyName("status")] public string Status { get; set; } = "ok";

        [JsonPropertyName("result")] public T Result { get; set; } = default!;
    }
}

