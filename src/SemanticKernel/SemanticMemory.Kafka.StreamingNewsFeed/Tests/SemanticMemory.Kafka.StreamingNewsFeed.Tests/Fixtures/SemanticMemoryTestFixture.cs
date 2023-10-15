using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class SemanticMemoryTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        services
            .AddDomain(configuration)
            ;
    }
}
