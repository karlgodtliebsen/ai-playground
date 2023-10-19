using AI.VectorDatabase.Qdrant.VectorStorage.Models;

namespace AI.VectorDatabases.MemoryStore.QdrantSemanticKernelFactory;

public interface IQdrantSemanticKernelMemoryStoreFactory
{
    Task<IQdrantSemanticKernelMemoryStore> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default);
}
