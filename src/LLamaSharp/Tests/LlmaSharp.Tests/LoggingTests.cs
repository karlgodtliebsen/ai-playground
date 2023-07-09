using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using Serilog;

namespace LlamaSharp.Tests;

public sealed class LoggingTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> factory;

    public LoggingTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    public void Dispose()
    {
        Log.CloseAndFlush();
        factory.Dispose();
    }

    [Fact]
    public void EnsureThatLoggingConfigurationIsValid()
    {
        factory.Services.GetService<ILogger>().Should().NotBeNull();
        factory.Services.GetService<ILogger<Program>>().Should().NotBeNull();
    }
}
