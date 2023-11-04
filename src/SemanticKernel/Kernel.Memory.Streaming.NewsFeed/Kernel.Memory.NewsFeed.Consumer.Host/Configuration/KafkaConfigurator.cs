using Kernel.Memory.NewsFeed.Domain.Domain;
using Kernel.Memory.NewsFeed.Domain.Domain.Implementation;
using Kernel.Memory.NewsFeed.Host.HostServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel.Memory.NewsFeed.Host.Configuration;

public static class KafkaHostingConfigurator
{

    public static IServiceCollection AddKafkaConsumerHosts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<KafkaConsumerService>();
        services.AddTransient<IKafkaConsumer, KafkaStreamingConsumer>();
        return services;
    }

}
