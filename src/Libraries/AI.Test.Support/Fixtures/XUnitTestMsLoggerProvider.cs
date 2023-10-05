using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

public sealed class XUnitTestMsLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ITestOutputHelper output;
    private readonly bool useScopes;
    private IExternalScopeProvider? scopes;

    public XUnitTestMsLoggerProvider(ITestOutputHelper output, bool useScopes)
    {
        this.output = output;
        this.useScopes = useScopes;
    }

    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new XUnitTestMsLogger(WriteLine, scopes, categoryName, useScopes);
    }

    private void WriteLine(string message)
    {
        try
        {
            output.WriteLine(message);
        }
        catch
        {
            // swallow exceptions from disposed test output helpers
        }
    }

    public void Dispose()
    {
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopes = scopeProvider;
    }
}
