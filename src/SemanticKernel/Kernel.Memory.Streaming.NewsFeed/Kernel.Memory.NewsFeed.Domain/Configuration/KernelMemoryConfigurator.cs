using Kernel.Memory.NewsFeed.Domain.Domain;
using Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.ContentStorage;
using Microsoft.SemanticMemory.MemoryStorage;

using Serilog;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public static class KernelMemoryConfigurator
{
    /// <summary>
    /// AddKernelMemoryConfiguration
    /// WithCustomStorage             IContentStorage
    /// WithCustomEmbeddingGeneration ITextEmbeddingGeneration service
    /// WithCustomVectorDb            ISemanticMemoryVectorDb
    /// FromAppSettings()
    /// FromConfiguration(SemanticMemoryConfig config
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKernelMemory(this IServiceCollection services)
    {
        Log.Logger.Information("Setting up Kernel Memory");

        services.AddTransient<IEventsToSemanticMemory, EventsToSemanticMemory>();
        //services.AddTransient<IContentStorage, CustomContentStorage>();
        services.AddTransient<IContentStorage, OpenSearchCustomContentStorage>();

        //services.AddTransient<ISemanticMemoryVectorDb, SemanticMemoryVectorDb>();
        services.AddTransient<ISemanticMemoryVectorDb, Microsoft.SemanticMemory.MemoryStorage.Qdrant.QdrantMemory>();
        services.AddTransient<IMemoryStore, QdrantMemoryStore>();
        //services.AddTransient<ITextEmbeddingGeneration, >();


        var sp = services.BuildServiceProvider();
        var memoryClientBuilder = new MemoryClientBuilder(services);
        memoryClientBuilder
            .FromAppSettings()
             //FromConfiguration(SemanticMemoryConfig config
             .WithCustomStorage(sp.GetRequiredService<IContentStorage>())
            //.WithCustomEmbeddingGeneration(sp.GetRequiredService<ITextEmbeddingGeneration>())
            //.WithCustomVectorDb(sp.GetRequiredService<ISemanticMemoryVectorDb>())
            ;

        ISemanticMemoryClient client = memoryClientBuilder.Build();
        services.AddSingleton(client);

        Log.Logger.Information("Completed Adding Kernel Memory Configuration");
        return services;
    }
}
