using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using Microsoft.SemanticMemory.MemoryStorage;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain.QdrantFactory;

public interface IQdrantMemoryStoreFactoryForSemanticMemory
{
    Task<ISemanticMemoryVectorDb> Create(string collectionName, int vectorSize, string distance = Distance.COSINE, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default);
}
