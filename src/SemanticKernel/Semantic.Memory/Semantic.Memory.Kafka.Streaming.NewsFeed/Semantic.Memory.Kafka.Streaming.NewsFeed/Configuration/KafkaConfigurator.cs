using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Semantic.Memory.Kafka.Streaming.NewsFeed.Domain;
using Semantic.Memory.Kafka.Streaming.NewsFeed.HostServices;

namespace Semantic.Memory.Kafka.Streaming.NewsFeed.Configuration;

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
