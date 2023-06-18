using Serilog.Core;
using Serilog.Events;
using Xunit.Abstractions;

namespace AI.Domain.Tests;

public class TestOutputHelperSink : ILogEventSink
{

    readonly ITestOutputHelper writer;


    public TestOutputHelperSink(ITestOutputHelper writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        this.writer = writer!;
    }

    public void Emit(LogEvent logEvent)
    {
        writer.WriteLine(logEvent.RenderMessage());
    }
}
