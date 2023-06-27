using LLamaSharpApp.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);


var services = builder.Services;
services.AddLlmaConfiguration(builder.Configuration);
services.AddControllers();
services.AddOpenApi();

var app = builder.Build();


app.UseRouting();
app.UseOpenApi();
app.UseHttpsRedirection();


app.MapControllers();

app.Run();


/// <summary>
/// Partial part of Program to support web application factory during test
/// </summary>
// ReSharper disable once UnusedType.Global
public partial class Program
{
}
