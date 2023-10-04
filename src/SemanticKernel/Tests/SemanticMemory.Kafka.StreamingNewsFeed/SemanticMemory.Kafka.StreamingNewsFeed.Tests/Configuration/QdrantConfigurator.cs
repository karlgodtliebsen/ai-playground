using Microsoft.Extensions.DependencyInjection;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Tests.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        //services.AddTransient<IQdrantMemoryStore, QdrantMemoryStore>();
        //services.AddTransient<IMemoryStore, QdrantMemoryStore>();
        //services.AddSingleton<IQdrantMemoryStoreFactory, QdrantMemoryStoreFactory>();
        return services;
    }
}
