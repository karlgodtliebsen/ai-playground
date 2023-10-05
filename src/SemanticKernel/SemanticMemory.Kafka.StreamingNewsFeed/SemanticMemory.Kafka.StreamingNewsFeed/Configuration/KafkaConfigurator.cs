using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SemanticMemory.Kafka.StreamingNewsFeed.Domain;
using SemanticMemory.Kafka.StreamingNewsFeed.HostServices;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

public static class KafkaConfigurator
{

    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaConfiguration>(configuration.GetSection(KafkaConfiguration.SectionName));
        services.AddTransient<KafkaConsumer>();
        services.AddTransient<KafkaProducer>();
        return services;
    }

    public static IServiceCollection AddKafkaProducerHosts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<KafkaProducerService>();
        return services;
    }
    public static IServiceCollection AddKafkaConsumerHosts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<KafkaConsumerService>();
        return services;
    }

}
