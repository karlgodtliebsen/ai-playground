using System.Net;

using AI.Library.Configuration;
using AI.Library.Utils;

using Microsoft.IdentityModel.Logging;

using TensorFlowApp.WebAPI.Configuration;
using TensorFlowApp.WebAPI.Configuration.LibraryConfiguration;

const string ApplicationName = "TensorFlowApp.WebAPI";

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
    .AddWebApiConfiguration(configuration);

if (!env.IsDevelopment() && !env.IsEnvironment(HostingEnvironments.UsingReverseProxy))
{
    services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

services
    .AddControllers();
services
    .AddOpenApi()
    .AddHealthCheck();

var app = builder.Build();
await using (app)
{
    if (!env.IsEnvironment(HostingEnvironments.UsingReverseProxy))
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
    }
    app.UseRouting();
    app.UseOpenApi();
    app.MapHealthCheckAnonymous();
    app.MapControllers();
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

