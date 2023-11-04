using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticMemory.MemoryStorage;

namespace AI.VectorDatabases.MemoryStore.KernelMemorySupport;

public class QdrantKernelMemoryFactory : IQdrantKernelMemoryFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;

    public QdrantKernelMemoryFactory(IServiceProvider serviceProvider, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task<ISemanticMemoryVectorDb> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE,
        bool recreateCollection = true,
        bool storeOnDisk = false, CancellationToken cancellationToken = default)
    {
        var factory = serviceProvider.GetRequiredService<IQdrantFactory>();

        var f = await factory.Create(collectionName, vectorSize, distance, recreateCollection, storeOnDisk, cancellationToken);

        var qdrantStorage = serviceProvider.GetRequiredService<ISemanticMemoryVectorDb>();
        //qdrantStorage.SetClient(f);
        return qdrantStorage;
    }

}
