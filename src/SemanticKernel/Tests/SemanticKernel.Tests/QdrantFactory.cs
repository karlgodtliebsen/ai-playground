using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using Microsoft.Extensions.DependencyInjection;

namespace SemanticKernel.Tests;

public class QdrantFactory : IQdrantFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly IQdrantVectorDb client;
    private readonly QdrantOptions qdrantOptions;
    private readonly ILogger logger;

    public QdrantFactory(IServiceProvider serviceProvider, IQdrantVectorDb client, QdrantOptions qdrantOptions, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.client = client;
        this.qdrantOptions = qdrantOptions;
        this.logger = logger;
    }

    public async Task<IQdrantMemoryStore> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE, bool storeOnDisk = false, CancellationToken cancellationToken = default)
    {
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Collection {collectionName} deleted", collectionName),
            error => throw new QdrantException(error.Error)
        );

        var vectorParams = client.CreateParams(vectorSize, distance, storeOnDisk);
        result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded Creating Collection {collectionName}", collectionName),
            error => throw new QdrantException(error.Error)
        );

        var memoryStorage = serviceProvider.GetRequiredService<IQdrantMemoryStore>();
        memoryStorage.SetVectorSize(vectorSize);
        return memoryStorage;
    }

}