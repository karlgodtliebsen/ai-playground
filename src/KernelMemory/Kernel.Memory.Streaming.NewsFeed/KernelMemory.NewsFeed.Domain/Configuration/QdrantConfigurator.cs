using AI.VectorDatabases.MemoryStore.KernelMemorySupport;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.MemoryStorage;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public static class QdrantConfigurator
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services)
    {
        services.AddTransient<ISemanticMemoryVectorDb, SemanticMemoryVectorDb>();
        //services.AddSingleton<IQdrantKernelMemoryFactory, QdrantKernelMemoryFactory>();

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
