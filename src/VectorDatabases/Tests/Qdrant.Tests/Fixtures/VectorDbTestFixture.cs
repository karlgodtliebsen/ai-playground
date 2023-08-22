using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace Qdrant.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class VectorDbTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddQdrant(configuration);
        AddDockerSupport(services, configuration);
    }
}
