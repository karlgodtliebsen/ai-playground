using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Repositories;
using LLamaSharpApp.WebAPI.Domain.Repositories.Implementation;
using LLamaSharpApp.WebAPI.Domain.Services;
using LLamaSharpApp.WebAPI.Domain.Services.Implementations;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// WebApi Configuration 
/// </summary>
public static class WebApiConfigurator
{
    /// <summary>
    /// WebAPI options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, WebApiOptions options)
    {
        services
            .VerifyAndAddOptions(options)
            .AddDomain()
            .AddUtilities()
            .AddRepository();
        return services;
    }
    /// <summary>
    /// Add configuration from appsettings.json for the WebAPI parts (ie the not llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, Action<WebApiOptions>? options = null)
    {
        var configuredOptions = new WebApiOptions();
        options?.Invoke(configuredOptions);
        return services.AddWebApiConfiguration(configuredOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the WebAPI parts (ie the not llama model parts)
    /// If validation is not required, then just bind the options directly
    /// IConfigurationSection section = configuration.GetSection(sectionName);
    /// var section = section.GetSection(sectionName);
    ///services.AddOptions&lt;WebApiOptions&gt;().Bind(section);
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="webApiOptionsSectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, IConfiguration configuration, string? webApiOptionsSectionName = null)
    {
        if (webApiOptionsSectionName is null)
        {
            webApiOptionsSectionName = WebApiOptions.SectionName;
        }

        var options = configuration.GetSection(webApiOptionsSectionName).Get<WebApiOptions>()!;
        ArgumentNullException.ThrowIfNull(options);
        return services.AddWebApiConfiguration(options);
    }

    private static IServiceCollection VerifyAndAddOptions(this IServiceCollection services, WebApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.ModelStatePersistencePath);
        ArgumentNullException.ThrowIfNull(options.StatePersistencePath);
        services.AddSingleton<IOptions<WebApiOptions>>(new OptionsWrapper<WebApiOptions>(options));
        return services;
    }
    private static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddScoped<IUserIdProvider, UserIdProvider>()
            ;
        return services;
    }

    private static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services
            .AddTransient<ILlamaModelFactory, LlamaModelFactory>()
            .AddTransient<IOptionsService, OptionsService>()
            .AddTransient<IChatService, ChatService>()
            .AddTransient<IEmbeddingsService, EmbeddingsService>()
            .AddTransient<IExecutorService, ExecutorService>()
            .AddTransient<ITokenizationService, TokenizationService>()
            ;

        return services;
    }

    private static IServiceCollection AddRepository(this IServiceCollection services)//might be moved to repository project later on 
    {
        services
            .AddTransient<IModelStateRepository, ModelStateFileRepository>()
            .AddTransient<IUsersStateRepository, UsersStateFileRepository>();
        return services;
    }

}
