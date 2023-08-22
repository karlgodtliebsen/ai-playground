using AI.Test.Support.Fixtures;

using Azure.OpenAI.Tests.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azure.OpenAI.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class AzureOpenAITestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        base.AddServices(services, configuration);
        services.AddAzureOpenAI(configuration);
    }
}
