using AI.Test.Support.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Library.Tests.Support.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class DockerLaunchTestFixture : TestFixtureBase
{
    public string DatabaseConnectionString { get; private set; } = string.Empty;
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        base.AddServices(services, configuration);
        DatabaseConnectionString = configuration.GetValue<string>("ConnectionString") ?? string.Empty;
    }
}
