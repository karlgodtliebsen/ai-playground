﻿using AI.Test.Support.Fixtures;

using FluentAssertions;

using LlamaSharp.Tests.Fixtures;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.DomainServices;
using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Serilog;

using Xunit.Abstractions;

namespace LlamaSharp.Tests;

[Collection("LLamaSharp Collection")]
public sealed class TestOfDomainConfiguration : IDisposable
{
    private readonly HostApplicationFactory factory;
    private readonly IServiceProvider services;
    public TestOfDomainConfiguration(ITestOutputHelper output, LLamaSharpTestFixture fixture)
    {
        this.factory = fixture.WithOutputLogSupport<TestFixtureBase>(output).Build();
        this.services = factory.Services;
    }
    public void Dispose()
    {
        Log.CloseAndFlush();
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

    [Fact]
    public void VerifyLoggerRegistration()
    {
        services.GetRequiredService<ILogger<TestOfDomainConfiguration>>();
        services.GetService<ILogger>().Should().NotBeNull();
        services.GetService<ILoggerFactory>().Should().NotBeNull();
        services.GetService<ILogger<object>>().Should().NotBeNull();
    }

    [Fact]
    public void EnsureThatServiceConfigurationIsValid()
    {
        //string model = $"/projects/AI/LlamaModels/{expectedModel}";
        string modelPath = $"/projects/AI/LlamaModels";

        services.GetService<ILLamaFactory>().Should().NotBeNull();
        services.GetService<IOptionsService>().Should().NotBeNull();
        services.GetService<ICompositeService>().Should().NotBeNull();
        services.GetService<IChatService>().Should().NotBeNull();
        services.GetService<IEmbeddingsService>().Should().NotBeNull();
        services.GetService<IInteractiveExecutorService>().Should().NotBeNull();
        services.GetService<IContextStateRepository>().Should().NotBeNull();
        services.GetService<IUsersStateRepository>().Should().NotBeNull();

        //Options
        services.GetService<IOptions<LlamaRepositoryOptions>>().Should().NotBeNull();
        services.GetService<IOptions<LLamaModelOptions>>().Should().NotBeNull();
        services.GetService<IOptions<InferenceOptions>>().Should().NotBeNull();

        var options = services.GetRequiredService<IOptions<LLamaModelOptions>>().Value;
        options.ModelPath.Should().Contain(modelPath);
        File.Exists(options.ModelPath).Should().BeTrue();

        var iOptions = services.GetRequiredService<IOptions<InferenceOptions>>().Value;
        iOptions.AntiPrompts.Single().Should().Be("User:");
        iOptions.MaxTokens.Should().Be(1024);
    }


}
