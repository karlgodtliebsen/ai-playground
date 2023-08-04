using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace AI.Library.HttpUtils.LibraryConfiguration;

/// <summary>
/// OpenApi Options
/// </summary>
public sealed class OpenApiOptions
{
    /// <summary>
    /// DefaultSectionName (const)
    /// </summary>
    public const string SectionName = "OpenApi";

    /// <summary>
    /// Constructor for OpenApiOptions
    /// </summary>
    public OpenApiOptions()
    {
        SecurityScheme = DefaultSecurityScheme();
        Info = DefaultOpenApiInfo();
        SecurityRequirement = DefaultSecurityRequirement();
    }

    /// <summary>
    /// Create a default OpenApiInfo
    /// </summary>
    /// <returns></returns>
    public static OpenApiInfo DefaultOpenApiInfo()
    {
        return new OpenApiInfo()
        {
            Version = "v1",
            Title = "Generic model requests",
            Description = "ASP.NET Web API handling requests",
        };
    }

    /// <summary>
    /// Create a default OpenApiSecurityScheme
    /// </summary>
    /// <returns></returns>
    public static OpenApiSecurityScheme DefaultSecurityScheme()
    {
        return new OpenApiSecurityScheme
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
        };
    }

    /// <summary>
    /// Create a default OpenApiSecurityRequirement
    /// </summary>
    /// <returns></returns>
    public static OpenApiSecurityRequirement DefaultSecurityRequirement()
    {
        return new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        };
    }

    /// <summary>
    /// OpenApi SecurityRequirement
    /// </summary>
    public OpenApiSecurityRequirement? SecurityRequirement { get; set; }

    /// <summary>
    /// OpenApi SecurityScheme
    /// </summary>
    public OpenApiSecurityScheme? SecurityScheme { get; set; }
    /// <summary>
    /// OpenApi Info
    /// </summary>
    public OpenApiInfo? Info { get; set; }

    /// <summary>
    /// This property is used to define the security definition for the OpenApi
    /// If null no OpenApi security is added
    /// </summary>
    public string? SecurityDefinition { get; set; } = "JWT Authentication";

    /// <summary>
    /// This property is used to define the security definition for the OpenApi
    /// If null no OpenApi security is added
    /// </summary>
    public string? SecurityDefinitionName { get; set; } = "Bearer";

    /// <summary>
    /// Use Xml Documentation
    /// </summary>
    public bool UseXml { get; set; } = true;
}
