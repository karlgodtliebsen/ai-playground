using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class SemanticMemoryTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenAIConfiguration(configuration)
            //.AddQdrant(configuration)
            //.AddQdrantVectorStore()
            //.AddBing(configuration)
            //.AddHuggingFace(configuration)
            ;
        //AddDockerSupport(services, configuration);
    }
}
