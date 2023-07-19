using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;

namespace SemanticKernel.Tests.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        services.AddTransient<IQdrantMemoryStore, QdrantMemoryStore>();
        services.AddTransient<IMemoryStore, QdrantMemoryStore>();
        services.AddSingleton<IQdrantMemoryStoreFactory, QdrantMemoryStoreFactory>();
        return services;
    }
}
