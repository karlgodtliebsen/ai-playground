using AI.CaaP.Repository.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Serilog;

Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Migrations");
const string applicationName = "AI.CaaP.Repository";


//ObservabilityConfigurator.StartLogging(applicationName);

try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;


    Log.Logger.Information("Starting Adding Services to Application");
    services
        .AddDatabaseContext((options) =>
        {
            var o = builder.Configuration.GetSection(options.SectionName).Get<DatabaseConnectionOptions>();
            options.ConnectionString = o.ConnectionString;
            options.MigrationAssembly = o.MigrationAssembly;
            options.UseProvider = o.UseProvider;
        })
        ;
    Log.Logger.Information("Completed Adding Services to Application");

    //create/build the configured host
    var app = builder.Build();

    //ObservabilityConfigurator.LogFinalizedConfiguration(applicationName);

    app.Services.DestroyMigration();
    app.Services.UseMigration();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Error(ex, $"Application {applicationName} failed to start", applicationName);
    throw;
}
finally
{
    //ObservabilityConfigurator.StopLogging(applicationName);
}


public partial class Program
{

}
