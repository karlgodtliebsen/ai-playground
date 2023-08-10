using AI.Test.Support;
using AI.Test.Support.DockerSupport;
using AI.VectorDatabase.Qdrant.Configuration;

using FinancialAgents.Tests.Configuration;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;

using BingOptions = FinancialAgents.Tests.Configuration.BingOptions;
using HuggingFaceOptions = FinancialAgents.Tests.Configuration.HuggingFaceOptions;

namespace FinancialAgents.Tests.Fixtures;


public class FinancialAgentsTestFixture : TestFixtureBase
{
    public QdrantOptions QdrantOptions { get; private set; }
    public BingOptions BingOptions { get; private set; }
    public GoogleOptions GoogleOptions { get; private set; }
    public HuggingFaceOptions HuggingFaceOptions { get; private set; }

    public LlamaModelOptions LlamaModelOptions { get; private set; }
    public ILlamaModelFactory LlamaModelFactory { get; private set; }
    public string TestFilesPath { get; private set; }
    //public string SkillsPath { get; private set; }

    public FinancialAgentsTestFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services
                    .AddQdrant(configuration)
                    .AddBing(configuration)
                    .AddHuggingFace(configuration)
                    .AddLLamaDomain(configuration)
                    ;
                services.AddSingleton<TestContainerDockerLauncher>();
                var section = configuration.GetSection(DockerLaunchOptions.SectionName);
                services.AddOptions<DockerLaunchOptions>().Bind(section);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        TestFilesPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));
        //SkillsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills"));
        BingOptions = Factory.Services.GetRequiredService<IOptions<BingOptions>>().Value;
        QdrantOptions = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        HuggingFaceOptions = Factory.Services.GetRequiredService<IOptions<HuggingFaceOptions>>().Value;
        LlamaModelOptions = Factory.Services.GetRequiredService<IOptions<LlamaModelOptions>>().Value;
        LlamaModelFactory = Factory.Services.GetRequiredService<ILlamaModelFactory>();
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class FinancialAgentsWithDockerTestFixture : FinancialAgentsTestFixture, IDisposable
{
    // ReSharper disable once MemberCanBePrivate.Global
    public TestContainerDockerLauncher? Launcher { get; private set; } = default;

    public Func<bool> Launch { get; set; }

    /// <summary>
    /// Post Build Setup of Logging and Launcher that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    public override void Setup(ITestOutputHelper output)
    {
        base.Setup(output);
        Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
        Launcher.Start();
    }
    public void Dispose()
    {
        Launcher?.Stop();
    }
}

