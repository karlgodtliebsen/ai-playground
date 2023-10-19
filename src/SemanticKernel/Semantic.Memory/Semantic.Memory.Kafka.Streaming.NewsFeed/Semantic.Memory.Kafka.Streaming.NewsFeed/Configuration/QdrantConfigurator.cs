using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.MemoryStorage;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        //services.AddTransient<ISemanticMemoryVectorDb, QdrantMemoryStoreForSemanticMemory>();
        //services.AddSingleton<IQdrantMemoryStoreFactoryForSemanticMemory, QdrantMemoryStoreFactoryForSemanticMemory>();

        var sp = services.BuildServiceProvider();
        var memory = new MemoryClientBuilder(services)
                .WithCustomVectorDb(sp.GetRequiredService<ISemanticMemoryVectorDb>())
                .FromAppSettings()
                .Build()
            ;
        services.AddSingleton(memory);
        return services;
    }
}
