using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AI.VectorDatabases.MemoryStore.SemanticKernelSupport;

public class QdrantSemanticKernelMemoryStoreFactory : IQdrantSemanticKernelMemoryStoreFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;

    public QdrantSemanticKernelMemoryStoreFactory(IServiceProvider serviceProvider, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task<IQdrantSemanticKernelMemoryStore> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE,
        bool recreateCollection = true,
        bool storeOnDisk = false, CancellationToken cancellationToken = default)
    {
        var factory = serviceProvider.GetRequiredService<IQdrantFactory>();

        var f = await factory.Create(collectionName, vectorSize, distance, recreateCollection, storeOnDisk, cancellationToken);

        var qdrantStorage = serviceProvider.GetRequiredService<IQdrantSemanticKernelMemoryStore>();
        qdrantStorage.SetClient(f);
        return qdrantStorage;
    }

}
