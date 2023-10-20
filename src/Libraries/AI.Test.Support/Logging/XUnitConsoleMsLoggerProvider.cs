using AI.Test.Support.Fixtures;
using Microsoft.Extensions.Logging;

namespace AI.Test.Support.Logging;

public sealed class XUnitConsoleMsLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly TextWriter output;
    private readonly bool useScopes;
    private IExternalScopeProvider? scopes = default!;

    public XUnitConsoleMsLoggerProvider(TextWriter output, bool useScopes)
    {
        this.output = output;
        this.useScopes = useScopes;
    }

    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new XUnitTestMsLogger((s) => output.WriteLine(s), scopes, categoryName, useScopes);
    }

    public void Dispose()
    {
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        scopes = scopeProvider;
    }
}
