using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

namespace LlamaSharp.Tests;

public sealed class TestOfWebApplicationLogging : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> factory;

    public TestOfWebApplicationLogging(WebApplicationFactory<Program> factory)
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
