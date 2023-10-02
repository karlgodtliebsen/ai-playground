using AI.VectorDatabase.Qdrant.VectorStorage.Models;

namespace SemanticMemory.Tests.Domain;

public interface IQdrantMemoryStoreFactory
{
    Task<IQdrantMemoryStore> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default);
}
