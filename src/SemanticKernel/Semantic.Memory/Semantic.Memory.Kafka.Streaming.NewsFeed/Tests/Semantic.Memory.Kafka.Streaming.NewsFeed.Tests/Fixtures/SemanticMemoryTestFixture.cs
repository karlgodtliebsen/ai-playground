using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Semantic.Memory.Kafka.Streaming.NewsFeed.Configuration;

namespace Semantic.Memory.Kafka.Streaming.NewsFeed.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class SemanticMemoryTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        //System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        services
            .AddDomain(configuration)
            ;
    }
}
