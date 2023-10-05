using AI.VectorDatabases.MemoryStore.QdrantFactory;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;

namespace SemanticKernel.Tests.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {

        services.AddTransient<IQdrantMemoryStore, AI.VectorDatabases.MemoryStore.QdrantFactory.QdrantMemoryStore>();
        services.AddTransient<IMemoryStore, AI.VectorDatabases.MemoryStore.QdrantFactory.QdrantMemoryStore>();
        services.AddSingleton<IQdrantMemoryStoreFactory, QdrantMemoryStoreFactory>();

        return services;
    }
}
