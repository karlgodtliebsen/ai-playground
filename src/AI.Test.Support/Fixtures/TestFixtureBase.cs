using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

//public abstract class TestFixtureBase
//{
//    public ITestOutputHelper Output { get; protected set; }
//    public ILogger Logger { get; protected set; }

//    public Microsoft.Extensions.Logging.ILogger MsLogger { get; protected set; }

//    public HostApplicationFactory Factory { get; protected set; }

//    protected TestFixtureBase()
//    {
//    }

//    /// <summary>
//    /// Post Build Setup of Logging that depends on ITestOutputHelper
//    /// </summary>
//    /// <param name="output"></param>
//    public virtual void Setup(ITestOutputHelper output)
//    {
//        Output = output;
//        Factory.ConfigureLogging(output);
//        Logger = Factory.Logger();
//        MsLogger = Factory.MsLogger();
//    }
//}

public abstract class TestFixtureBase
{
    public string Environment { get; private set; } = "IntegrationTests";
    public DateTimeOffset DateTimeOffset { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// abstract constructor
    /// </summary>
    protected TestFixtureBase()
    {
        //Environment = "IntegrationTests";
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
