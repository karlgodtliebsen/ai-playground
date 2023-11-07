using System.Net;

using Kernel.Memory.NewsFeed.Domain.Domain;
using Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Polly;

using Serilog;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public static class DomainConfigurator
{
    /// <summary>
    /// AddOpenAIConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddNewsFeedDomain(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Setting up using Applications Configuration");
        services
            .AddKafka(configuration)
            .AddOpenSearch(configuration)
            .AddKernelMemory()
            .AddHttpClient()
        ;

        services.AddHttpClient<IStreamingClient, StreamingClient>()
            .AddPolicyHandler(GetCircuitBreakerPolicyForCustomerServiceNotFound())
        ;
        Log.Logger.Information("Completed Adding Applications Configuration");
        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicyForCustomerServiceNotFound()
    {
        return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound)
                .CircuitBreakerAsync(1, TimeSpan.FromMicroseconds(1))
            ;
    }

}
