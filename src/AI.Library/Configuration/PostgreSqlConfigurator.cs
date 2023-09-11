using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AI.Library.Configuration;


/// <summary>
/// Configures/wires PostgreSql Configuration Support
/// </summary>
public static class PostgreSqlConfigurator
{
    /// <summary>
    /// Add PostgreSql Options Support
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddPostgreSql(this IServiceCollection services, IConfiguration configuration, Action<PostgreSqlOptions>? options = null, string? sectionName = null)
    {
        sectionName ??= PostgreSqlOptions.SectionName;
        var pSqlOptions = configuration.GetSection(sectionName).Get<PostgreSqlOptions>();
        if (pSqlOptions is null)
        {
            pSqlOptions = new PostgreSqlOptions();
        }
        options?.Invoke(pSqlOptions);
        ArgumentNullException.ThrowIfNull(pSqlOptions.ConnectionString);
        services.AddSingleton(Options.Create(pSqlOptions));
        return services;
    }
}
