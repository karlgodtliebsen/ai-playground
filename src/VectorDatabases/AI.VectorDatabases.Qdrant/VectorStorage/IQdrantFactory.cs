using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

public interface IQdrantFactory
{
    VectorParams CreateParams(int? dimension = null, string? distance = null, bool? storeOnDisk = null);

    Task<IQdrantVectorDb> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default);

    Task<IQdrantVectorDb> Create(string collectionName, VectorParams vectorParams,
        bool recreateCollection = true,
        bool storeOnDisk = false, CancellationToken cancellationToken = default);

}
