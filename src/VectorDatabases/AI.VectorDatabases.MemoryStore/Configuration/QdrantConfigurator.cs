using AI.VectorDatabases.MemoryStore.QdrantSemanticKernelFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;

namespace AI.VectorDatabases.MemoryStore.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        services.AddTransient<IQdrantSemanticKernelMemoryStore, QdrantSemanticKernelMemoryStore>();
        services.AddTransient<IMemoryStore, QdrantSemanticKernelMemoryStore>();
        services.AddSingleton<IQdrantSemanticKernelMemoryStoreFactory, QdrantSemanticKernelMemoryStoreFactory>();
        return services;
    }
}
