using Serilog.Core;
using Serilog.Events;

using Xunit.Abstractions;

namespace AI.Test.Support;

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
        writer.WriteLine(logEvent.RenderMessage());
    }
}
