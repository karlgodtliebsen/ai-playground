using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace AI.Test.Support;

public class OutputSink : ILogEventSink
{
    readonly ITextFormatter _formatter;
    readonly TextWriter? writer;


    public OutputSink(TextWriter writer, ITextFormatter formatter)
    {
        ArgumentNullException.ThrowIfNull(writer);
        this.writer = writer!;
        _formatter = formatter;
    }

    public void Emit(LogEvent logEvent)
    {
        _formatter.Format(logEvent, writer!);
    }
}
