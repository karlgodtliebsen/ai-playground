using Xunit.Abstractions;

namespace AI.Test.Support;

public abstract class TestFixtureBase
{
    public ITestOutputHelper Output { get; protected set; }
    public ILogger Logger { get; protected set; }
    public Microsoft.Extensions.Logging.ILogger MsLogger { get; protected set; }

    public HostApplicationFactory Factory { get; protected set; }

    protected TestFixtureBase()
    {
    }

    /// <summary>
    /// Post Build Setup of Logging that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    public virtual void Setup(ITestOutputHelper output)
    {
        Output = output;
        Factory.ConfigureLogging(output);
        Logger = Factory.Logger();
        MsLogger = Factory.MsLogger();
    }
}
