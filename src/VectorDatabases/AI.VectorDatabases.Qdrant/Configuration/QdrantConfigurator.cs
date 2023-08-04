using AI.VectorDatabase.Qdrant.VectorStorage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AI.VectorDatabase.Qdrant.Configuration;

public static class QdrantConfigurator
{
    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddQdrant(this IServiceCollection services, Action<QdrantOptions>? options = null)
    {
        var configuredOptions = new QdrantOptions();
        options?.Invoke(configuredOptions);
        return services.AddQdrant(configuredOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddQdrant(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= QdrantOptions.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<QdrantOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddQdrant(configuredOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the Llma parts (ie the llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddQdrant(this IServiceCollection services, IConfiguration configuration, Action<QdrantOptions> options, string? sectionName = null)
    {
        sectionName ??= QdrantOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<QdrantOptions>() ?? new QdrantOptions();
        options.Invoke(modelOptions);
        return services.AddQdrant(modelOptions);
    }

}
