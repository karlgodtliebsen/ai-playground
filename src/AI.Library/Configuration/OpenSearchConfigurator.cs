using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AI.Library.Configuration;

/// <summary>
/// Configures/wires PostgreSql Configuration Support
/// </summary>
public static class OpenSearchConfigurator
{
    /// <summary>
    /// Add PostgreSql Options Support
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration, Action<OpenSearchOptions>? options = null, string? sectionName = null)
    {
        sectionName ??= OpenSearchOptions.SectionName;
        var openSearchOptions = configuration.GetSection(sectionName).Get<OpenSearchOptions>();
        if (openSearchOptions is null)
        {
            openSearchOptions = new OpenSearchOptions();
        }
        options?.Invoke(openSearchOptions);
        services.AddSingleton(Options.Create(openSearchOptions));
        return services;
    }
}
