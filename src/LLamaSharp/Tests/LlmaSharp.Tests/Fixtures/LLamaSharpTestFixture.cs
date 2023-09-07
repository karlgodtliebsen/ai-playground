using AI.Test.Support.Fixtures;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaSharp.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class LLamaSharpTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddLLamaConfiguration(configuration)
            .AddLLamaDomain(configuration)
            .AddInferenceConfiguration(configuration)
            .AddLLamaRepository(configuration)
            ;
        //AddDockerSupport(services, configuration);
    }
}
