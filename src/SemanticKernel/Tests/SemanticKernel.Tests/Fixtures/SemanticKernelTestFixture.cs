using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Configuration;

namespace SemanticKernel.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class SemanticKernelTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddQdrant(configuration)
            .AddQdrantVectorStore()
            .AddOpenAIConfiguration(configuration)
            .AddAzureOpenAI(configuration)
            .AddBing(configuration)
            .AddHuggingFace(configuration)
            ;
        AddDockerSupport(services, configuration);
    }
}
