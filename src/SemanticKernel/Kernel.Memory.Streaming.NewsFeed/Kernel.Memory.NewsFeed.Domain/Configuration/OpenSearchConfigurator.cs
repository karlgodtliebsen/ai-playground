using Kernel.Memory.NewsFeed.Domain.Util.OpenSearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenSearch.Client;
using OpenSearch.Net;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public static class OpenSearchConfigurator
{

    /// <summary>
    /// Add configuration from app settings.json for the OpenSearch configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, OpenSearchConfiguration options)
    {
        services.AddSingleton(Options.Create(options));
        services.AddTransient<IOpenSearchAdminClient, OpenSearchAdminClient>();
        services.AddSingleton(BuildOpenSearchClient(options));
        services.AddSingleton(BuildOpenSearchLowLevelClient(options));
        return services;
    }

    /// <summary>
    /// Add configuration from app settings.json for the OpenSearch configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, Action<OpenSearchConfiguration>? options = null)
    {
        var configuredOptions = new OpenSearchConfiguration();
        options?.Invoke(configuredOptions);
        return services.AddOpenSearch(configuredOptions);
    }

    /// <summary>
    /// Add configuration from app settings.json for the OpenSearch configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= OpenSearchConfiguration.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenSearchConfiguration>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddOpenSearch(configuredOptions);
    }

    /// <summary>
    /// Add configuration from app settings.json for the OpenSearch configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration, Action<OpenSearchConfiguration> options, string? sectionName = null)
    {
        sectionName ??= OpenSearchConfiguration.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<OpenSearchConfiguration>() ?? new OpenSearchConfiguration();
        options.Invoke(modelOptions);
        return services.AddOpenSearch(modelOptions);
    }

    private static IOpenSearchClient BuildOpenSearchClient(OpenSearchConfiguration options)
    {
        var connectionPool = new SingleNodeConnectionPool(new Uri(options.EndPoint));
        var connectionSettings = new ConnectionSettings(connectionPool)
                //.RequestTimeout(TimeSpan.FromSeconds(4))
                //.MaxRetryTimeout(TimeSpan.FromSeconds(12))
                //.DefaultIndex(indexName)
                .DisableDirectStreaming(true)
                .BasicAuthentication(options.UserName, options.Password)
                .ServerCertificateValidationCallback((a, b, c, d) => true)
            ;
        var client = new OpenSearchClient(connectionSettings);

        return client;
    }


    /*
    Registering OpenSearch.Client as a singleton
    As a rule, you should set up your OpenSearch.Client as a singleton.
    OpenSearch.Client manages connections to the server and the states of the nodes in a cluster.
    Additionally, each openSearchLowLevelClient uses a lot of configuration for its setup.
    Therefore, it is beneficial to create an OpenSearch.Client instance once and reuse it for all OpenSearch operations.
    The openSearchLowLevelClient is thread safe, so the same instance can be shared by multiple threads.
    */

    private static IOpenSearchLowLevelClient BuildOpenSearchLowLevelClient(OpenSearchConfiguration options)
    {
        var connectionPool = new SingleNodeConnectionPool(new Uri(options.EndPoint));
        var connectionSettings = new ConnectionSettings(connectionPool)
                //.RequestTimeout(TimeSpan.FromSeconds(4))
                //.MaxRetryTimeout(TimeSpan.FromSeconds(12))
                //.DefaultIndex(indexName)
                .DisableDirectStreaming(true)
                .EnableHttpCompression()
                .PrettyJson()
                .ServerCertificateValidationCallback((a, b, c, d) => true)
                .BasicAuthentication(options.UserName, options.Password)
            ;

        var client = new OpenSearchLowLevelClient(connectionSettings);
        return client;
    }

}
