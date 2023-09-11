using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using FinancialAgents.Tests.Fixtures;

using FluentAssertions;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.DomainServices;
using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;

namespace FinancialAgents.Tests;

[Collection("FinancialAgents Collection")]
public sealed class TestOfConfiguration : IClassFixture<FinancialAgentsTestFixture>
{
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;

    public TestOfConfiguration(ITestOutputHelper output, FinancialAgentsTestFixture fixture)
    {
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).Build();
        this.services = hostApplicationFactory.Services;
    }

    [Fact]
    public void EnsureThatServiceConfigurationIsValid()
    {
        services.GetService<ILogger>().Should().NotBeNull();

        services.GetService<ILoggerFactory>().Should().NotBeNull();
        services.GetService<ILogger<object>>().Should().NotBeNull();

        services.GetService<ILLamaFactory>().Should().NotBeNull();
        services.GetService<IOptionsService>().Should().NotBeNull();
        services.GetService<IChatService>().Should().NotBeNull();
        services.GetService<IEmbeddingsService>().Should().NotBeNull();
        services.GetService<IInteractiveExecutorService>().Should().NotBeNull();
        services.GetService<ICompositeService>().Should().NotBeNull();
        services.GetService<IContextStateRepository>().Should().NotBeNull();
        services.GetService<IUsersStateRepository>().Should().NotBeNull();

        //Options
        services.GetService<IOptions<LlamaRepositoryOptions>>().Should().NotBeNull();
        services.GetService<IOptions<LLamaModelOptions>>().Should().NotBeNull();
        services.GetService<IOptions<InferenceOptions>>().Should().NotBeNull();

    }
}
