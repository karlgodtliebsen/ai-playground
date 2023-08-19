using System.Text;

using Microsoft.Extensions.Logging;

namespace AI.Test.Support.Fixtures;

public sealed class XUnitTestMsLogger : Microsoft.Extensions.Logging.ILogger
{
    private const string ScopeDelimiter = "=> ";
    private const string Spacer = "      ";

    private const string Trace = "trce";
    private const string Debug = "dbug";
    private const string Info = "info";
    private const string Warn = "warn";
    private const string Error = "fail";
    private const string Critical = "crit";

    private readonly string categoryName;
    private readonly bool useScopes;
    private readonly Action<string> write;
    private readonly IExternalScopeProvider? scopes;

    public XUnitTestMsLogger(Action<string> write, IExternalScopeProvider? scopes, string categoryName, bool useScopes)
    {
        this.write = write;
        this.scopes = scopes;
        this.categoryName = categoryName;
        this.useScopes = useScopes;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        if (!useScopes || scopes is null)
        {
            return null;
        }
        return scopes!.Push(state);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        var sb = new StringBuilder();

        switch (logLevel)
        {
            case LogLevel.Trace:
                sb.Append(Trace);
                break;
            case LogLevel.Debug:
                sb.Append(Debug);
                break;
            case LogLevel.Information:
                sb.Append(Info);
                break;
            case LogLevel.Warning:
                sb.Append(Warn);
                break;
            case LogLevel.Error:
                sb.Append(Error);
                break;
            case LogLevel.Critical:
                sb.Append(Critical);
                break;
            case LogLevel.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }

        sb.Append(": ").Append(categoryName).Append('[').Append(eventId).Append(']').AppendLine();

        if (useScopes && TryAppendScopes(sb))
            sb.AppendLine();

        sb.Append(Spacer);
        sb.Append(formatter(state, exception));

        if (exception is not null)
        {
            sb.AppendLine();
            sb.Append(Spacer);
            sb.Append(exception);
        }

        var message = sb.ToString();
        write(message);
    }

    private bool TryAppendScopes(StringBuilder sb)
    {
        var isScoped = false;
        if (!useScopes || scopes is null)
        {
            return isScoped;
        }
        this.scopes!.ForEachScope((callback, state) =>
        {
            if (!isScoped)
            {
                state.Append(Spacer);
                isScoped = true;
            }

            state.Append(ScopeDelimiter);
            state.Append(callback);
        }, sb);
        return isScoped;
    }
}
