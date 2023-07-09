﻿using FluentAssertions;

using LlamaSharp.Tests.Utils;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Repositories;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LlamaSharp.Tests;

public sealed class ConfigurationTests : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly IntegrationTestWebApplicationFactory factory;

    public ConfigurationTests(IntegrationTestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    public void Dispose()
    {
        factory.Dispose();
    }

    [Fact]
    public void EnsureThatServiceConfigurationIsValid()
    {
        factory.Services.GetService<ILogger>().Should().NotBeNull();
        factory.Services.GetService<IOptions<WebApiOptions>>().Should().NotBeNull();
        factory.Services.GetService<IOptions<LlamaModelOptions>>().Should().NotBeNull();
        factory.Services.GetService<IOptions<InferenceOptions>>().Should().NotBeNull();
        factory.Services.GetService<ILlamaModelFactory>().Should().NotBeNull();
        factory.Services.GetService<IOptionsService>().Should().NotBeNull();
        factory.Services.GetService<IChatService>().Should().NotBeNull();
        factory.Services.GetService<IEmbeddingsService>().Should().NotBeNull();
        factory.Services.GetService<IExecutorService>().Should().NotBeNull();
        factory.Services.GetService<IModelStateRepository>().Should().NotBeNull();
        factory.Services.GetService<IUsersStateRepository>().Should().NotBeNull();
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetService<IUserIdProvider>().Should().NotBeNull();

    }

}
