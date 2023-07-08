using AI.CaaP.Repositories;
using AI.CaaP.Repository.DatabaseContexts;
using AI.CaaP.Repository.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.CaaP.Repository.Configuration;

public static class RepositoryConfigurator
{

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddTransient<IConversationRepository, ConversationRepository>();
        services.AddTransient<IAgentRepository, AgentRepository>();
        return services;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, Action<DatabaseConnectionOptions> options = null)
    {
        var dbOptions = new DatabaseConnectionOptions()
        {
            ConnectionString = ""
        };
        options?.Invoke(dbOptions);
        services.AddDatabaseContext(dbOptions);
        return services;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, DatabaseConnectionOptions dbOptions)
    {
        services.AddDbContext<ConversationDbContext>(options =>
        {
            switch (dbOptions.UseProvider)
            {
                case "mssql":
                    options.UseSqlServer(dbOptions.ConnectionString, b => b.MigrationsAssembly(dbOptions.MigrationAssembly));
                    break;
                case "postgres":
                    options.UseNpgsql(dbOptions.ConnectionString, b => b.MigrationsAssembly(dbOptions.MigrationAssembly));
                    break;
            }
            options.EnableDetailedErrors(true);
            options.EnableSensitiveDataLogging(false);
        });
        return services;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = DatabaseConnectionOptions.ConfigSectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<DatabaseConnectionOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddDatabaseContext(configuredOptions);
    }


    public static void CleanDatabase(this IServiceProvider services)
    {
        var scope = services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<ConversationDbContext>();
        db.Clean();
    }

    public static void UseMigration(this IServiceProvider services)
    {
        var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ConversationDbContext>();
        context.Database.Migrate();
    }

    public static void DestroyMigration(this IServiceProvider services)
    {
        var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ConversationDbContext>();
        context.Database.ExecuteSqlRaw("""
            
            DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR

            SET @Cursor = CURSOR FAST_FORWARD FOR
            SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_SCHEMA + '].[' +  tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + '];'
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
            LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME

            OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql

            WHILE (@@FETCH_STATUS = 0)
            BEGIN
             Exec sp_executesql @Sql
             FETCH NEXT FROM @Cursor INTO @Sql
            END

            CLOSE @Cursor DEALLOCATE @Cursor;
            

            EXEC sp_MSforeachtable 'DROP TABLE ?';
            
            """);
    }

}
