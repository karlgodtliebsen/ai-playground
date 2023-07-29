using AI.Library.Configuration;

using Microsoft.IdentityModel.Logging;

using ML.TensorFlowApp.WebAPI.Configuration;
using ML.TensorFlowApp.WebAPI.Configuration.LibraryConfiguration;

const string Origins = "AllowedOrigins";
const string ApplicationName = "ML.TensorFlowApp.WebAPI";


Observability.StartLogging(ApplicationName);
var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;
IdentityModelEventSource.ShowPII = env.IsDevelopment();

builder.WithLogging();
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.AddServerHeader = false;
//});

services
    .AddWebApiConfiguration(configuration)
    //.AddAzureAdConfiguration(configuration, (string?)null)
    //.AddCorsConfig(configuration, options =>
    //{
    //    options.Policy = Origins;
    //    options.Origins = new[] { "https://localhost:7042" };
    //})
    //.AddControllers(options =>
    //{
    //    var policy = new AuthorizationPolicyBuilder()
    //        .RequireAuthenticatedUser()
    //        .Build();
    //    options.Filters.Add(new AuthorizeFilter(policy));
    //})
    ;

services
    .AddControllers();

services
    .AddOpenApi()
    .AddHealthCheck();

var app = builder.Build();
await using (app)
{
    //app.UseSecurityHeaders(SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));
    //app.UseCors(Origins);

    app.UseRouting();
    app.UseOpenApi();
    app.UseHttpsRedirection();

    //app.UseAuthentication();
    //app.UseAuthorization();

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

