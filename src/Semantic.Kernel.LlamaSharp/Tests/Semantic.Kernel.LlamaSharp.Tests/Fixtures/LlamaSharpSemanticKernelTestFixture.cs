using AI.Test.Support.DockerSupport;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabases.MemoryStore.Configuration;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SemanticKernel.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class LlamaSharpSemanticKernelTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddQdrant(configuration)
            .AddQdrantVectorStore()

            //.AddOpenAIConfiguration(configuration)
            //.AddAzureOpenAI(configuration)
            //.AddBing(configuration)
            //.AddHuggingFace(configuration)
            .AddLLamaConfiguration(configuration)
            .AddLLamaDomain(configuration)
            .AddInferenceConfiguration(configuration)
            .AddLLamaRepository(configuration)
            ;

        //AddDockerSupport(services, configuration);
    }

}
