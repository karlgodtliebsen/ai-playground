using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using Microsoft.SemanticMemory.MemoryStorage;

namespace AI.VectorDatabases.MemoryStore.KernelMemorySupport;

public interface IQdrantKernelMemoryFactory
{
    Task<ISemanticMemoryVectorDb> Create(string collectionName, int vectorSize, string distance = Distance.COSINE, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default);
}
