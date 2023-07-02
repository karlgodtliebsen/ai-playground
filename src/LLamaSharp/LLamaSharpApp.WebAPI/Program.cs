using LLamaSharpApp.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);


var env = builder.Environment;
var services = builder.Services;
services
    .AddConfiguration(builder.Configuration)
    .AddLlmaConfiguration(builder.Configuration);


services.AddControllers();

services.AddOpenApi();
services.AddHealthCheck();

var app = builder.Build();

app.UseRouting();
app.UseOpenApi();
app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

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
