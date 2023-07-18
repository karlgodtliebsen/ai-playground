using AI.VectorDatabase.Qdrant.VectorStorage.Models;

namespace SemanticKernel.Tests;

public interface IQdrantFactory
{
    Task<IQdrantMemoryStore> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE, bool storeOnDisk = false, CancellationToken cancellationToken = default);
}