using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Serilog;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

public class QdrantFactory : IQdrantFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly IQdrantClient qdrantClient;
    private readonly QdrantOptions options;
    private readonly ILogger logger;

    public QdrantFactory(IServiceProvider serviceProvider, IQdrantClient qdrantClient, IOptions<QdrantOptions> qdrantOptions, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.qdrantClient = qdrantClient;
        this.options = qdrantOptions.Value;
        this.logger = logger;
    }
    public VectorParams CreateParams(int? dimension = null, string? distance = null,
        bool? storeOnDisk = null)
    {
        if (string.IsNullOrEmpty(distance))
        {
            distance = options.DefaultDistance;
        }
        if (!storeOnDisk.HasValue)
        {
            storeOnDisk = options.DefaultStoreOnDisk;
        }

        if (!dimension.HasValue)
        {
            dimension = options.DefaultDimension;
        }
        var p = new VectorParams(size: dimension.Value, distance: distance, storeOnDisk.Value);
        return p;
    }

    public async Task<IQdrantClient> Create(string collectionName, int vectorSize,
        string distance = Distance.COSINE,
        bool recreateCollection = true,
        bool storeOnDisk = false, CancellationToken cancellationToken = default)
    {
        await RecreateIfSpecified(collectionName, vectorSize, distance, recreateCollection, storeOnDisk);

        var memoryStorage = serviceProvider.GetRequiredService<IQdrantClient>();
        memoryStorage.SetCollectionName(collectionName);
        memoryStorage.SetVectorSize(vectorSize);
        return memoryStorage;
    }

    private async Task RecreateIfSpecified(string collectionName, int vectorSize, string distance, bool recreateCollection, bool storeOnDisk)
    {
        if (recreateCollection)
        {
            var result = await qdrantClient.RemoveCollection(collectionName, CancellationToken.None);
            result.Switch(
                _ => logger.Information("Collection {collectionName} deleted", collectionName),
                error => throw new QdrantException(error.Error)
            );

            var vectorParams = CreateParams(vectorSize, distance, storeOnDisk);
            result = await qdrantClient.CreateCollection(collectionName, vectorParams, CancellationToken.None);
            result.Switch(
                _ => logger.Information("Succeeded Creating Collection {collectionName}", collectionName),
                error => throw new QdrantException(error.Error)
            );
        }
    }


    public async Task<IQdrantClient> Create(string collectionName, VectorParams vectorParams, bool recreateCollection = true, bool storeOnDisk = false, CancellationToken cancellationToken = default)
    {
        await RecreateIfSpecified(collectionName, vectorParams.Size, vectorParams.Distance, recreateCollection, storeOnDisk);
        var memoryStorage = serviceProvider.GetRequiredService<IQdrantClient>();
        memoryStorage.SetCollectionName(collectionName);
        memoryStorage.SetVectorSize(vectorParams.Size);
        return memoryStorage;
    }
}
