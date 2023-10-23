namespace AI.Test.Support.Logging;

public class XUnitTestLogger : Microsoft.Extensions.Logging.ILogger
{

    private readonly ILogger? logger;

    /// <summary>
    /// Creates a new <see cref="XUnitTestLoggerFactory"/> instance.
    /// </summary>
    public XUnitTestLogger(ILogger logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return XUnitTestLoggerScope.Instance;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Emit(formatter(state, exception));
    }

    public void Emit(string message)
    {
        logger.Debug(message);
    }

}