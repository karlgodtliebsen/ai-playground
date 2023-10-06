using AI.VectorDatabase.Qdrant.VectorStorage.Models;

namespace AI.VectorDatabases.MemoryStore.QdrantFactory;

public interface IQdrantMemoryStoreFactoryForSemanticKernel
{
    Task<IQdrantMemoryStoreForSemanticKernel> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default);
}
