﻿namespace LLamaSharpApp.WebAPI.Configuration;

/// <summary>
/// Handles OpenAPI (Swagger) configuration.
/// </summary>
public static class OpenApiConfigurator
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    /// <summary>
    /// Add OpenAPI (Swagger support) to the pipeline.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName);
            var name = AppDomain.CurrentDomain.FriendlyName;
            var xmlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{name}.xml", SearchOption.TopDirectoryOnly).ToList();
            xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
        });
        return services;
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
            app.UseSwaggerUI();
        }
        return app;
    }
}