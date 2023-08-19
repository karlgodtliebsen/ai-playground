using System.Data.Common;

using AI.CaaP.Configuration;
using AI.CaaP.Repository.Configuration;
using AI.CaaP.Repository.DatabaseContexts;
using AI.Test.Support.Fixtures;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

namespace AI.Caap.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class CaapWithDatabaseTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        services
            .AddCaaP(configuration)
            .AddOpenAIConfiguration(configuration)
            .AddRepository()
            .AddDatabaseContext(configuration)
            ;
        AddDockerSupport(services, configuration);
    }
}


// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CaapWithInMemoryDatabaseTestFixture : CaapWithDatabaseTestFixture
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        base.AddServices(services, configuration);
        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ConversationDbContext>));
        if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);
        var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
        if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);
        services.AddDbContext<ConversationDbContext>(options =>
        {
            options.UseInMemoryDatabase("Conversations");
        });
    }
}

