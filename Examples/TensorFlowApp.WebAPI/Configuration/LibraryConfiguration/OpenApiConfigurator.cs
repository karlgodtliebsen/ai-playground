using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace TensorFlowApp.WebAPI.Configuration.LibraryConfiguration;

/// <summary>
/// Handles OpenAPI (Swagger) configuration.
/// </summary>
public static class OpenApiConfigurator
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    /// <summary>
    /// Add OpenAPI (Swagger support) to the pipeline. This implementation is quite application specific as it hold names
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TensorFlow model requests",
                Description = "ASP.NET Web API handling TensorFlow requests",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "JWT Authentication",
                Type = SecuritySchemeType.Http,
                Description = "Enter JWT Bearer token **_only_**",
                BearerFormat = "JWT",
                Scheme = "bearer", // must be lower case
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            });

            var requirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    }, new string[] { }
                }
            };
            options.AddSecurityRequirement(requirement);
            options.CustomSchemaIds(type => type.FullName);
            options.SchemaFilter<EnumSchemaFilter>();
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
