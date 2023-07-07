using AI.Library.Qdrant.VectorStorage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using QdrantCSharp;

namespace AI.Library.Qdrant.Configuration;

public static class QdrantConfigurator
{

    public static IServiceCollection AddQdrant(this IServiceCollection services, QdrantOptions options)
    {
        services.AddSingleton<IOptions<QdrantOptions>>(new OptionsWrapper<QdrantOptions>(options));
        services.AddScoped<IVectorDb, QdrantDb>();
        //services.AddScoped<IAgentService, AgentService>();

        services.AddTransient<QdrantHttpClient>(_ => new QdrantHttpClient(url: options.QdrantUrl, apiKey: "n/a"));

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
