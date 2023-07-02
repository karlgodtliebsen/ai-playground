using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Configuration.LibraryConfiguration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;

const string Origins = "AllowedOrigins";

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;
//builder.AddEnvironmentVariables();
//builder.AddUserSecrets<HostApplicationFactory>();

IdentityModelEventSource.ShowPII = env.IsDevelopment();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

services
    .AddWebApiConfiguration(configuration)
    .AddLlmaConfiguration(configuration)
    .AddAzureAdConfiguration(configuration, (string?)null);

services.AddCorsConfig(configuration, options =>
{
    options.Policy = Origins;
    options.Origins = new[] { "https://localhost:7039" };
});

services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

services.AddOpenApi();
services.AddHealthCheck();

var app = builder.Build();

app.UseSecurityHeaders(SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));
app.UseCors(Origins);

app.UseRouting();
app.UseOpenApi();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthCheckAnonymous();
app.MapControllers();

app.Run();


/// <summary>
/// Partial part of Program to support web application factory during test
/// </summary>
// ReSharper disable once UnusedType.Global
public partial class Program
{
}
