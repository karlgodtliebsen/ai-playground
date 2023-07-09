using System.Net;
using AI.VectorDatabase.Qdrant.VectorStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace AI.VectorDatabase.Qdrant.Configuration;

public static class QdrantConfigurator
{

    public static IServiceCollection AddQdrant(this IServiceCollection services, QdrantOptions options)
    {
        services.AddSingleton<IOptions<QdrantOptions>>(new OptionsWrapper<QdrantOptions>(options));
        services.AddHttpClient<IVectorDb, QdrantDb>((_, client) =>
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
            sectionName = QdrantOptions.ConfigSectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<QdrantOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddQdrant(configuredOptions);
    }

}

public static class HttpClientsPolicies
{

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicyForNotFound()
    {
        return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound)
                .CircuitBreakerAsync(1, TimeSpan.FromMicroseconds(1))
            ;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
