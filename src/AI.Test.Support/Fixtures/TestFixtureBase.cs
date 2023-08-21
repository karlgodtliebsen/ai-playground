using AI.Test.Support.DockerSupport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

public abstract class TestFixtureBase
{
    private string Environment { get; set; } = "IntegrationTests";
    private bool useLogging = false;
    private bool useDocker = false;
    private ITestOutputHelper? output;

    public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;

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

    private HostApplicationFactory BuildWithoutLogging()
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
    protected void AddDockerSupport(IServiceCollection services, IConfiguration cfg)
    {
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = cfg.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }
    protected virtual void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        if (useDocker)
        {
            AddDockerSupport(services, configuration!);
        }
    }

    //support for builder pattern
    private HostApplicationFactory BuildFactory()
    {
        if (useLogging && output is not null)
        {
            return BuildWithLogging(output!);
        }
        return BuildWithoutLogging();
    }

    public TestFixtureBase WithDockerSupport()
    {
        useDocker = true;
        return this;
    }

    public virtual TestFixtureBase WithOutputLogSupport(ITestOutputHelper outputHelper)
    {
        useLogging = true;
        this.output = outputHelper;
        return this;
    }

    public virtual HostApplicationFactory Build()
    {
        var factory = BuildFactory();
        if (useDocker)
        {
            factory = factory.WithDockerSupport();
        }
        return factory;
    }
}
