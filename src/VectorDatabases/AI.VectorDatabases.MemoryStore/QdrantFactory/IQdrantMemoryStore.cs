using AI.VectorDatabase.Qdrant.VectorStorage;

using Microsoft.SemanticKernel.Memory;

namespace AI.VectorDatabases.MemoryStore.QdrantFactory;

public interface IQdrantMemoryStore : IMemoryStore
{
    /// <summary>
    /// Get a MemoryRecord using PointId
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="pointId"></param>
    /// <param name="withVector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MemoryRecord?> GetWithPointIdAsync(string collectionName, string pointId, bool withVector = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initialize by adding a late created client that cannot be transferred via constructor
    /// </summary>
    /// <param name="qdrantVectorDb"></param>
    void SetClient(IQdrantClient qdrantVectorDb);
}
