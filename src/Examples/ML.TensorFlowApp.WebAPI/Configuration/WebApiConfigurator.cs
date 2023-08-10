using Microsoft.Extensions.Options;

using ML.TensorFlowApp.WebAPI.Domain.Services;

namespace ML.TensorFlowApp.WebAPI.Configuration;

/// <summary>
/// WebApi Configuration 
/// </summary>
public static class WebApiConfigurator
{
    /// <summary>
    /// WebAPI options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, IConfiguration configuration, WebApiOptions options)
    {
        services
            .AddSingleton<IOptions<WebApiOptions>>(new OptionsWrapper<WebApiOptions>(options))
            //.AddMlnetImageClassification(configuration)
            .AddTransient<IImageClassifierService, ImageClassifierService>()
            //    .AddHttpContextAccessor()
            //    .AddScoped<IUserIdProvider, UserIdProvider>()
            ;
        return services;
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
        return services.AddWebApiConfiguration(configuration, options);
    }
}
