using Kernel.Memory.NewsFeed.Domain.Util.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public static class KafkaConfigurator
{
    public static IServiceCollection AddKafka(this IServiceCollection services, KafkaConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(Options.Create(options));
        services.AddTransient<KafkaAdminClient>();
        return services;
    }


    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(KafkaConfiguration.SectionName).Get<KafkaConfiguration>()!;
        ArgumentNullException.ThrowIfNull(options);
        return services.AddKafka(options);
    }


    public static IServiceCollection AddKafka(this IServiceCollection services, Action<KafkaConfiguration>? options = null)
    {
        var configuredOptions = new KafkaConfiguration();
        options?.Invoke(configuredOptions);
        return services.AddKafka(configuredOptions);
    }

    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= KafkaConfiguration.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<KafkaConfiguration>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddKafka(configuredOptions);
    }
}
