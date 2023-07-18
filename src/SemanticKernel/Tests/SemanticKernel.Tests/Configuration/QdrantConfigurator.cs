using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;

namespace SemanticKernel.Tests.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        services.AddTransient<IQdrantMemoryStore, QdrantVectorDbStore>();
        services.AddTransient<IMemoryStore, QdrantVectorDbStore>();
        services.AddSingleton<IQdrantFactory, QdrantFactory>();
        return services;
    }
}
