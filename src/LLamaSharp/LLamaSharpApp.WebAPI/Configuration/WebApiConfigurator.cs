using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Repositories;
using LLamaSharpApp.WebAPI.Repositories.Implementation;
using LLamaSharpApp.WebAPI.Services;
using LLamaSharpApp.WebAPI.Services.Implementations;

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
    public static IServiceCollection AddConfiguration(this IServiceCollection services, WebApiOptions options)
    {
        services
            .VerifyAndAddOptions(options)
            .AddDomain()
            .AddUtilities()
            .AddRepository();
        return services;
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
            .AddTransient<IUserIdProvider, UserIdProvider>()    // will end up being http request context scoped
            .AddTransient<ILlmaModelFactory, LlmaModelFactory>()
            .AddTransient<IOptionsService, OptionsService>()
            ;
        return services;
    }
    private static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services
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

    /// <summary>
    /// Add configuration from appsettings.json for the WebAPI parts (ie the not llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfiguration(this IServiceCollection services, Action<WebApiOptions>? options = null)
    {
        var configuredOptions = new WebApiOptions();
        options?.Invoke(configuredOptions);
        return services.AddConfiguration(configuredOptions);
    }

    /// <summary>
    /// Add configuration from appsettings.json for the WebAPI parts (ie the not llama model parts)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="webapiOptionsSectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration, string? webapiOptionsSectionName = null)
    {
        if (webapiOptionsSectionName is null)
        {
            webapiOptionsSectionName = WebApiOptions.SectionName;
        }
        //If vaidation is not required, then just bind the options directly
        //IConfigurationSection section = configuration.GetSection(webapiOptionsSectionName);
        //var section = section.GetSection(webapiOptionsSectionName);
        //services.AddOptions<WebApiOptions>().Bind(section);

        var options = configuration.GetSection(webapiOptionsSectionName).Get<WebApiOptions>()!;
        return services.AddConfiguration(options);
    }
}
