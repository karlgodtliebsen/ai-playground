using AI.CaaP.Configuration;
using AI.CaaP.Repository.Configuration;
using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

namespace AI.Caap.Tests.Fixtures;

public class CaapWithDatabaseTestFixture : TestFixtureBaseWithDocker
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCaaP(configuration)
            .AddOpenAIConfiguration(configuration)
            .AddRepository()
            .AddDatabaseContext(configuration)
            ;
    }
}
