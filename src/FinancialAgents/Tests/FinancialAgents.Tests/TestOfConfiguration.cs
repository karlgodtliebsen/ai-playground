using AI.Test.Support;

using FinancialAgents.Tests.Fixtures;

using FluentAssertions;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialAgents.Tests;

public sealed class TestOfConfiguration : IClassFixture<FinancialAgentsTestFixture>
{
    private readonly FinancialAgentsTestFixture factory;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfConfiguration(FinancialAgentsTestFixture factory)
    {
        this.factory = factory;
        this.hostApplicationFactory = factory.Factory;
    }


    [Fact]
    public void EnsureThatServiceConfigurationIsValid()
    {
        hostApplicationFactory.Services.GetService<ILogger>().Should().NotBeNull();

        hostApplicationFactory.Services.GetService<ILoggerFactory>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<ILogger<object>>().Should().NotBeNull();

        hostApplicationFactory.Services.GetService<ILlamaModelFactory>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IOptionsService>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IChatDomainService>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IEmbeddingsService>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IExecutorService>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IModelStateRepository>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IUsersStateRepository>().Should().NotBeNull();

        //Options
        hostApplicationFactory.Services.GetService<IOptions<LlamaRepositoryOptions>>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IOptions<LlamaModelOptions>>().Should().NotBeNull();
        hostApplicationFactory.Services.GetService<IOptions<InferenceOptions>>().Should().NotBeNull();

    }
}
