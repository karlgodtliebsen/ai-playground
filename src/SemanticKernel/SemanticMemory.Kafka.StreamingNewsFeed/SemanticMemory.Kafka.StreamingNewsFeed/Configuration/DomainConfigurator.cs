using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticMemory;

using SemanticMemory.Kafka.StreamingNewsFeed.Domain;

using Serilog;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

public static class DomainConfigurator
{
    /// <summary>
    /// AddOpenAIConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Setting up using Applications Configuration");

        services.AddKafka(configuration)
            .AddKafkaProducerHosts(configuration)
            .AddKafkaConsumerHosts(configuration)
            .AddQdrant(configuration)
            .AddQdrantVectorStore()
            .AddTransient<IEventsToSemanticMemory, EventsToSemanticMemory>()
            ;
        ISemanticMemoryClient memory = new MemoryClientBuilder(services)
            .FromAppSettings()
            .Build();

        //WithCustomStorage             IContentStorage
        //WithCustomEmbeddingGeneration ITextEmbeddingGeneration service
        //WithCustomVectorDb            ISemanticMemoryVectorDb
        //FromConfiguration(SemanticMemoryConfig config

        services.AddSingleton(memory);
        Log.Logger.Information("Completed Adding Applications Configuration");
        return services;
    }
    public static HostApplicationBuilder AddSecrets<T>(this HostApplicationBuilder builder) where T : class
    {
        builder.Configuration.AddUserSecrets<T>();
        return builder;
    }

    //public static IServiceCollection AddSimpleFileStorageAsContentStorage(this IServiceCollection services, SimpleFileStorageConfig config)
    //{
    //    return services
    //        .AddSingleton<SimpleFileStorageConfig>(config)
    //        .AddSingleton<IContentStorage, CustomContentStorage>();
    //}
}
