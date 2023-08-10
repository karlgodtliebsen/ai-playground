using AI.CaaP.WebAPI.Configuration;
using AI.Library.Configuration;
using AI.Library.HttpUtils.LibraryConfiguration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;

const string Origins = "AllowedOrigins";
const string ApplicationName = "AI.CaaP.WebAPI";

Observability.UseBootstrapLogger(ApplicationName);

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;
IdentityModelEventSource.ShowPII = env.IsDevelopment();

builder.WithLogging();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});
services

    .AddWebApiConfiguration(configuration, (string?)null)
    .AddAzureAdConfiguration(configuration, (string?)null)
    .AddCors()
    .AddControllers(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });
services
    .AddSecurity(configuration)
    .AddOpenApi()
    .AddHealthCheck()
    ;

var app = builder.Build();
await using (app)
{
    app.UseSecurityHeaders(SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));
    app.UseCors(Origins);

    app.UseRouting();
    app.UseOpenApi();
    app.UseHttpsRedirection();

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

