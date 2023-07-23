using Serilog.Core;
using Serilog.Events;

using Xunit.Abstractions;

namespace AI.Test.Support.Logging;

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
        try
        {
            writer.WriteLine(logEvent.RenderMessage());
        }
        catch (Exception)
        {
            Console.WriteLine(logEvent.RenderMessage());
        }
    }
}
