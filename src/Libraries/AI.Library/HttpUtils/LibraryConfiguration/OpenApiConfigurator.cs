using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace AI.Library.HttpUtils.LibraryConfiguration;

/// <summary>
/// Handles OpenAPI (Swagger) configuration.
/// </summary>
public static class OpenApiConfigurator
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    /// <summary>
    /// Add OpenAPI (Swagger support)
    /// </summary>
    /// <param name="services"></param>
    /// <param name="openApiOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services, OpenApiOptions openApiOptions)
    {
        services.AddSingleton<IOptions<OpenApiOptions>>(Options.Create(openApiOptions));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            if (openApiOptions.SecurityDefinition is not null)
            {
                openApiOptions.Info ??= OpenApiOptions.DefaultOpenApiInfo();
                openApiOptions.SecurityScheme ??= OpenApiOptions.DefaultSecurityScheme();
                openApiOptions.SecurityRequirement ??= OpenApiOptions.DefaultSecurityRequirement();

                options.SwaggerDoc(openApiOptions!.Info!.Version, openApiOptions.Info);
                options.AddSecurityDefinition(openApiOptions.SecurityDefinitionName, openApiOptions.SecurityScheme);
                options.AddSecurityRequirement(openApiOptions.SecurityRequirement);
                options.CustomSchemaIds(type => type.FullName);
                options.SchemaFilter<EnumSchemaFilter>();
                if (openApiOptions.UseXml)
                {
                    var name = AppDomain.CurrentDomain.FriendlyName;
                    var xmlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{name}.xml", SearchOption.TopDirectoryOnly).ToList();
                    xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
                }
            }
        });
        return services;
    }


    /// <summary>
    /// Add Default configuration 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services, Action<OpenApiOptions>? options = null)
    {
        var configuredOptions = new OpenApiOptions();
        options?.Invoke(configuredOptions);
        return services.AddOpenApi(configuredOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default sectionname (OpenApi) or the provided sectionname
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        sectionName ??= OpenApiOptions.SectionName;
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenApiOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddOpenApi(configuredOptions);
    }

    /// <summary>
    /// Add configuration from configuration using default sectionname (OpenApi) or the provided sectionname
    /// and allows for overriding of the configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration configuration, Action<OpenApiOptions> options, string? sectionName = null)
    {
        sectionName ??= OpenApiOptions.SectionName;
        var modelOptions = configuration.GetSection(sectionName).Get<OpenApiOptions>() ?? new OpenApiOptions();
        options.Invoke(modelOptions);
        return services.AddOpenApi(modelOptions);
    }


    /// <summary>
    /// Adds Swagger and SwaggerUI to the pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseOpenApi(this WebApplication app)
    {
        return app.UseOpenApi(() => true);
    }

    /// <summary>
    /// Adds Swagger and SwaggerUI to the pipeline when .
    /// </summary>
    /// <param name="app"></param>
    /// <param name="restrictToEnvironment">Func to enable filtering</param>
    /// <returns></returns>
    public static WebApplication UseOpenApi(this WebApplication app, Func<bool> restrictToEnvironment)
    {
        if (restrictToEnvironment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.EnableDeepLinking();
                options.DocExpansion(DocExpansion.List);
                options.DisplayRequestDuration();
            });
        }
        return app;
    }
}
