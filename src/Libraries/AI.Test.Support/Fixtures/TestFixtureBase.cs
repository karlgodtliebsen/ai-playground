using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

public abstract class TestFixtureBase
{
    protected string Environment { get; set; } = "IntegrationTests";
    protected bool useLogging = false;
    protected ITestOutputHelper? output;

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
    /// <param name="outputHelper"></param>
    protected HostApplicationFactory BuildWithLogging(ITestOutputHelper outputHelper)
    {
        var factory = HostApplicationFactory.Build(
            outputHelper,
            environment: AddEnvironment,
            serviceContext: AddServices,
            fixedDateTime: AddTestDateTime);
        return factory;
    }

    protected HostApplicationFactory BuildWithoutLogging()
    {
        var factory = HostApplicationFactory.Build(
            environment: AddEnvironment,
            serviceContext: AddServices,
            fixedDateTime: AddTestDateTime);
        return factory;
    }
    protected virtual void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    protected virtual string AddEnvironment()
    {
        return Environment;
    }

    protected virtual DateTimeOffset AddTestDateTime()
    {
        return DateTimeOffset;
    }

    //support for builder pattern
    protected virtual HostApplicationFactory BuildFactory()
    {
        if (useLogging && output is not null)
        {
            return BuildWithLogging(output!);
        }
        return BuildWithoutLogging();
    }

    public virtual T WithOutputLogSupport<T>(ITestOutputHelper outputHelper) where T : TestFixtureBase
    {
        useLogging = true;
        this.output = outputHelper;
        return (T)this;
    }

    public virtual HostApplicationFactory Build()
    {
        var factory = BuildFactory();
        return factory;
    }
}
