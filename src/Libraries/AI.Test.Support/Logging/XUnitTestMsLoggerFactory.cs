using Microsoft.Extensions.Logging;

namespace AI.Test.Support.Logging;

/// <summary>
/// XUnitTestLoggerFactory
/// An <see cref="ILoggerFactory"/> used to create instance of XUnitTestLogger
/// </summary>
public sealed class XUnitTestMsLoggerFactory : ILoggerFactory
{
    private readonly ILoggerProvider loggerProvider;

    /// <summary>
    /// Creates a new <see cref="XUnitTestMsLoggerFactory"/> instance.
    /// </summary>
    public XUnitTestMsLoggerFactory(ILoggerProvider loggerProvider)
    {
        this.loggerProvider = loggerProvider;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This returns a <see cref="XUnitTestMsLogger"/> instance which logs nothing.
    /// </remarks>
    public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
    {
        return loggerProvider.CreateLogger(name);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method ignores the parameter and does nothing.
    /// </remarks>
    public void AddProvider(ILoggerProvider provider)
    {
    }
    /// <inheritdoc />
    public void Dispose()
    {
    }
}
