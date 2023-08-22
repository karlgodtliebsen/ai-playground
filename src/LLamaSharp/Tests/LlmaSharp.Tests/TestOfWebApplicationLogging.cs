using FluentAssertions;

using LlamaSharp.Tests.Fixtures;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using Xunit.Abstractions;

namespace LlamaSharp.Tests;

public sealed class TestOfWebApplicationLogging : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly WebApplicationFactory<Program> factory;

    public TestOfWebApplicationLogging(IntegrationTestWebApplicationFactory factory, ITestOutputHelper output)
    {
        this.factory = factory.WithOutputLogSupport(output).Build<IntegrationTestWebApplicationFactory>();
    }

    public void Dispose()
    {
        Log.CloseAndFlush();
    }

    [Fact]
    public void EnsureThatLoggingConfigurationIsValid()
    {
        factory.Services.GetService<ILogger>().Should().NotBeNull();
        factory.Services.GetService<ILogger<Program>>().Should().NotBeNull();
    }

    [Fact]
    public void ShowThatMicrosoftTestLoggerEmitsMessage()
    {
        var logger = factory.Services.GetRequiredService<ILogger<TestOfDomainConfiguration>>();
        logger.Should().NotBeNull();
        logger.LogInformation("Log output message Microsoft Logger");
    }

    [Fact]
    public void ShowThatSerilogTestLoggerEmitsMessage()
    {
        var logger = factory.Services.GetRequiredService<ILogger>();
        logger.Should().NotBeNull();
        logger.Information("Log output message Serilog");
    }

}
