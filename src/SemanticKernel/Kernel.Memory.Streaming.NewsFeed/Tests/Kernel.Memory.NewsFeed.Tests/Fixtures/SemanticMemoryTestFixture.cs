using AI.Test.Support.DockerSupport;
using Kernel.Memory.NewsFeed.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel.Memory.NewsFeed.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class SemanticMemoryTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        //System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        services
            .AddNewsFeedDomain(configuration)
            ;
    }
}
