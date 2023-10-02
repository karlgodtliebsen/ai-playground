using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;
using SemanticMemory.Tests.Domain;

namespace SemanticMemory.Tests.Configuration;

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
