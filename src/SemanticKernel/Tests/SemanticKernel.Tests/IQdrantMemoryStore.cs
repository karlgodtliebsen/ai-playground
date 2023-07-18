using Microsoft.SemanticKernel.Memory;

namespace SemanticKernel.Tests;

public interface IQdrantMemoryStore : IMemoryStore
{
    Task<MemoryRecord?> GetWithPointIdAsync(string collectionName, string pointId, bool withEmbedding = false, CancellationToken cancellationToken = default);
    void SetVectorSize(int vectorSize);
}
