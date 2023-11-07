using System.Net;

using AI.Library.Configuration;
using AI.Library.HttpUtils.LibraryConfiguration;
using AI.Library.Utils;

using Microsoft.IdentityModel.Logging;

using Semantic.Memory.WebAPI.Configuration;

const string Origins = "AllowedOrigins";
const string ApplicationName = "SemanticMemory.WebAPI";

Observability.UseBootstrapLogger(ApplicationName);

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;
IdentityModelEventSource.ShowPII = env.IsDevelopment();

builder.WithLogging();

if (!env.IsEnvironment(HostingEnvironments.UsingReverseProxy))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.AddServerHeader = false;
    });
}
services
    .AddWebApiConfiguration(configuration)
    .AddCors(Origins)
    ;

if (!env.IsDevelopment() && !env.IsEnvironment(HostingEnvironments.UsingReverseProxy))
{
    services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

services
    .AddSecurity(configuration)
    .AddOpenApi(configuration)
    .AddHealthCheck()
    ;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
await using (app)
{
    app.MapEndpoints();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseOpenApi();
    app.UseCors(Origins);

    if (!env.IsEnvironment(HostingEnvironments.UsingReverseProxy))
    {
        app.UseSecurityHeaders(SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHealthCheckAnonymous();
    Observability.LogFinalizedConfiguration(ApplicationName);

    await app.RunAsync();
}
Observability.StopLogging(ApplicationName);


/// <summary>
/// Partial part of Program to support web application factory during test
/// </summary>
// ReSharper disable once UnusedType.Global
public partial class Program
{
}

