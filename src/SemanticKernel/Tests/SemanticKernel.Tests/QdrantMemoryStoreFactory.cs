using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SemanticKernel.Tests;

public class QdrantMemoryStoreFactory : IQdrantMemoryStoreFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly IQdrantVectorDb client;
    private readonly QdrantOptions qdrantOptions;
    private readonly ILogger logger;

    public QdrantMemoryStoreFactory(IServiceProvider serviceProvider, IQdrantVectorDb client, IOptions<QdrantOptions> qdrantOptions, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.client = client;
        this.qdrantOptions = qdrantOptions.Value;
        this.logger = logger;
    }

    public async Task<IQdrantMemoryStore> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE,
        bool recreateCollection = true,
        bool storeOnDisk = false, CancellationToken cancellationToken = default)
    {
        IQdrantFactory factory = serviceProvider.GetRequiredService<IQdrantFactory>();

        var f = await factory.Create(collectionName, vectorSize, distance, recreateCollection, storeOnDisk, cancellationToken);

        var memoryStorage = serviceProvider.GetRequiredService<IQdrantMemoryStore>();
        memoryStorage.SetClient(f);
        //memoryStorage.Initialize(collectionName, vectorSize);
        return memoryStorage;
    }

}
