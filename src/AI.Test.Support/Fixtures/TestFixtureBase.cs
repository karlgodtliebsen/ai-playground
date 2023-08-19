using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

public abstract class TestFixtureBase
{
    public string Environment { get; private set; } = "IntegrationTests";
    public DateTimeOffset DateTimeOffset { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// abstract constructor
    /// </summary>
    protected TestFixtureBase()
    {
        //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0
        var env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (env is not null)
        {
            Environment = env;
        }
    }

    protected void AddDockerSupport(IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = configuration.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }

    /// <summary>
    /// Post Build Setup of Logging that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    private HostApplicationFactory BuildWithLogging(ITestOutputHelper output)
    {
        var factory = HostApplicationFactory.Build(
            output,
            environment: AddEnvironment,
            serviceContext: AddServices,
            fixedDateTime: AddTestDateTime);
        return factory;
    }

    private HostApplicationFactory Build()
    {
        var factory = HostApplicationFactory.Build(
            environment: AddEnvironment,
            serviceContext: AddServices,
            fixedDateTime: AddTestDateTime);
        return factory;
    }

    protected virtual string AddEnvironment()
    {
        return Environment;
    }

    protected virtual DateTimeOffset AddTestDateTime()
    {
        return DateTimeOffset;
    }

    protected virtual void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
    }

    public virtual HostApplicationFactory BuildFactoryWithLogging(ITestOutputHelper output)
    {
        var factory = BuildWithLogging(output);
        return factory;
    }
    public virtual HostApplicationFactory BuildFactory()
    {
        var factory = Build();
        return factory;
    }
}
