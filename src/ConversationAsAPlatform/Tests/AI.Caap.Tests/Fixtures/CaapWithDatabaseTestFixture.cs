using AI.CaaP.Configuration;
using AI.CaaP.Repository.Configuration;
using AI.Test.Support.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

namespace AI.Caap.Tests.Fixtures;

public class CaapWithDatabaseTestFixture : TestFixtureBase
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
