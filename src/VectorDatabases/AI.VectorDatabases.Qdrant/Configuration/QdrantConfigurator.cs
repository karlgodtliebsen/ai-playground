using AI.VectorDatabase.Qdrant.VectorStorage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AI.VectorDatabase.Qdrant.Configuration;

public static class QdrantConfigurator
{

    public static IServiceCollection AddQdrant(this IServiceCollection services, QdrantOptions options)
    {
        services.AddTransient<IQdrantFactory, QdrantFactory>();
        services.AddSingleton<IOptions<QdrantOptions>>(new OptionsWrapper<QdrantOptions>(options));
        services.AddHttpClient<IQdrantVectorDb, QdrantVectorDb>((_, client) =>
            {
                client.BaseAddress = new Uri(options.Url);
            })
        .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
        .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());
        return services;
    }

    public static IServiceCollection AddQdrant(this IServiceCollection services, Action<QdrantOptions>? options = null)
    {
        var configuredOptions = new QdrantOptions();
        options?.Invoke(configuredOptions);
        return services.AddQdrant(configuredOptions);
    }

    public static IServiceCollection AddQdrant(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = QdrantOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<QdrantOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddQdrant(configuredOptions);
    }

}
