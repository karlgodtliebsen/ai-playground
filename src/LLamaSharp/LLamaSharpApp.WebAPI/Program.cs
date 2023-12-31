﻿using System.Net;

using AI.Library.Configuration;
using AI.Library.HttpUtils.LibraryConfiguration;
using AI.Library.Utils;

using LLamaSharp.Domain.Configuration;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;

const string Origins = "AllowedOrigins";
const string ApplicationName = "LLamaSharpApp.WebAPI";

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
    .AddLLamaConfiguration(configuration)
    .AddInferenceConfiguration(configuration)
    .AddCors(Origins)
    .AddControllers(
            options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }
        )
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

var app = builder.Build();
await using (app)
{
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseRouting();
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
