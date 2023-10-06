using AI.VectorDatabases.MemoryStore.QdrantFactory;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;

namespace AI.VectorDatabases.MemoryStore.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        services.AddTransient<IQdrantMemoryStoreForSemanticKernel, QdrantMemoryStoreForSemanticKernel>();
        services.AddTransient<IMemoryStore, QdrantMemoryStoreForSemanticKernel>();
        services.AddSingleton<IQdrantMemoryStoreFactoryForSemanticKernel, QdrantMemoryStoreFactoryForSemanticKernel>();
        return services;
    }
}
