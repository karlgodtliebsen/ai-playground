using AI.Test.Support.DockerSupport;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabases.MemoryStore.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

using SemanticMemory.Tests.Configuration;


namespace SemanticMemory.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class SemanticMemoryTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddQdrant(configuration)
            .AddQdrantVectorStore()
            .AddOpenAIConfiguration(configuration)
            //.AddAzureOpenAI(configuration)
            .AddBing(configuration)
            .AddHuggingFace(configuration)
            ;
        AddDockerSupport(services, configuration);
    }
}
