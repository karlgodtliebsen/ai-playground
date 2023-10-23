using AI.VectorDatabase.Qdrant.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Semantic.Memory.Kafka.Streaming.NewsFeed.Domain;
using Serilog;

namespace Semantic.Memory.Kafka.Streaming.NewsFeed.Configuration;

public static class DomainConfigurator
{
    /// <summary>
    /// AddOpenAIConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Setting up using Applications Configuration");

        services
            .AddKafka(configuration)
            .AddKafkaProducerHosts(configuration)
            .AddKafkaConsumerHosts(configuration)
            .AddQdrant(configuration)
            .AddQdrantVectorStore()
            .AddTransient<IEventsToSemanticMemory, EventsToSemanticMemory>()
            ;

        //WithCustomStorage             IContentStorage
        //WithCustomEmbeddingGeneration ITextEmbeddingGeneration service
        //WithCustomVectorDb            ISemanticMemoryVectorDb
        //FromConfiguration(SemanticMemoryConfig config

        Log.Logger.Information("Completed Adding Applications Configuration");
        return services;
    }


    public static HostApplicationBuilder AddSecrets<T>(this HostApplicationBuilder builder) where T : class
    {
        builder.Configuration.AddUserSecrets<T>();
        return builder;
    }
}
