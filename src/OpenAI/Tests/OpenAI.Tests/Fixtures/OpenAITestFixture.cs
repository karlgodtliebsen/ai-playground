using AI.Test.Support.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;

namespace OpenAI.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class OpenAITestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        base.AddServices(services, configuration);
        services.AddOpenAIConfiguration(configuration);
    }
}
