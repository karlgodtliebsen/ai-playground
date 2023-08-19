using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

namespace Embeddings.Qdrant.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class EmbeddingsVectorDbTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        services
            .AddQdrant(configuration)
            .AddOpenAIConfiguration(configuration);
        AddDockerSupport(services, configuration);
    }

}
