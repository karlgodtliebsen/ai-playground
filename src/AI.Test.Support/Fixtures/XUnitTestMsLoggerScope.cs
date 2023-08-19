namespace AI.Test.Support.Fixtures;

public sealed class XUnitTestMsLoggerScope : IDisposable
{
    public static XUnitTestMsLoggerScope Instance { get; } = new XUnitTestMsLoggerScope();

    private XUnitTestMsLoggerScope()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
