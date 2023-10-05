using System.Diagnostics;

using Serilog.Core;
using Serilog.Events;

using Xunit.Abstractions;

namespace AI.Test.Support.Logging;

public class LazyTestOutputHelperSink : ILogEventSink
{
    private readonly Func<ITestOutputHelper>? outputHelperProvider;

    public LazyTestOutputHelperSink(Func<ITestOutputHelper> outputHelperProvider)
    {
        this.outputHelperProvider = outputHelperProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        var writer = outputHelperProvider!();
        if (writer is null)
        {
            Debug.Print(logEvent.RenderMessage());
            return;
        }
        writer.WriteLine(logEvent.RenderMessage());
    }
}
