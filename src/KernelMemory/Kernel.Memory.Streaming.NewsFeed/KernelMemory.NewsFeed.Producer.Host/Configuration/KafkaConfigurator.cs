using Kernel.Memory.NewsFeed.Domain.Domain;
using Kernel.Memory.NewsFeed.Domain.Domain.Implementation;
using Kernel.Memory.NewsFeed.Producer.Host.HostServices;
using Kernel.Memory.NewsFeed.Producer.Host.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel.Memory.NewsFeed.Producer.Host.Configuration;

public static class KafkaHostingConfigurator
{

    public static IServiceCollection AddKafkaProducerHosts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<KafkaProducerService>();
        services.AddTransient<IKafkaProducer, KafkaProducer>();
        return services;
    }


}
