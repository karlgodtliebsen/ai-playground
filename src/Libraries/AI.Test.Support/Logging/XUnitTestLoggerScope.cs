namespace AI.Test.Support.Logging;

internal sealed class XUnitTestLoggerScope : IDisposable
{
    public static XUnitTestLoggerScope Instance { get; } = new XUnitTestLoggerScope();

    private XUnitTestLoggerScope()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}