using AI.Test.Support.Fixtures;

using FinancialAgents.Tests.Configuration;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAgents.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class FinancialAgentsTestFixture : TestFixtureBase
{

    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        services
            .AddHuggingFace(configuration)
            .AddSearchConfiguration(configuration)
            .AddHuggingFace(configuration)
            .AddLLamaDomain(configuration)
            ;
        AddDockerSupport(services, configuration);
    }
}
