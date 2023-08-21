using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Library.Tests.Support.Tests.Fixtures;

public class DockerLaunchTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = configuration.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }
}
