using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;

using Embeddings.Qdrant.Tests.Fixtures;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;

using SemanticKernel.Tests.Configuration;

using Xunit.Abstractions;

namespace SemanticKernel.Tests.Fixtures;

public sealed class SemanticKernelTestFixture : SemanticKernelTestFixtureBase, IDisposable
{


    public TestContainerDockerLauncher Launcher { get; private set; }
    public Func<bool> Launch { get; set; }

    public SemanticKernelTestFixture()
    {
        Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
        Launcher.Start();
    }

    public void Dispose()
    {
        Launcher.Stop();
    }
}

public class SemanticKernelTestFixtureBase
{
    public ILogger Logger { get; private set; }
    public Microsoft.Extensions.Logging.ILogger MsLogger { get; set; }


    public HostApplicationFactory Factory { get; private set; }
    public AzureOpenAIOptions AzureOpenAIOptions { get; private set; }
    public OpenAIOptions OpenAIOptions { get; private set; }
    public QdrantOptions QdrantOptions { get; private set; }
    public BingOptions BingOptions { get; private set; }

    public LlamaModelOptions LlamaModelOptions { get; private set; }
    public IModelRequestFactory RequestFactory { get; private set; }
    public ILlamaModelFactory LlamaModelFactory { get; private set; }
    public string TestFilesPath { get; private set; }

    private readonly Func<ITestOutputHelper>? getOutput;
    public ITestOutputHelper Output { get; set; }

    public TestContainerDockerLauncher Launcher { get; private set; }
    public Func<bool> Launch { get; set; }

    public SemanticKernelTestFixtureBase()
    {
        getOutput = () => Output!;
        Factory = HostApplicationFactory.Build(
             environment: () => "IntegrationTests",
             serviceContext: (services, configuration) =>
             {
                 services
                     .AddQdrant(configuration)
                     .AddQdrantVectorStore()
                     .AddOpenAIConfiguration(configuration)
                     .AddAzureOpenAI(configuration)
                     .AddBing(configuration)
                     .AddLlamaConfiguration(configuration)
                     .AddWebApiConfiguration(configuration)
                     ;
                 services.AddSingleton<TestContainerDockerLauncher>();
                 var section = configuration.GetSection(DockerLaunchOptions.SectionName);
                 services.AddOptions<DockerLaunchOptions>().Bind(section);
             },
             fixedDateTime: () => DateTimeOffset.UtcNow,
             output: getOutput
         );
        Logger = Factory.Logger();
        MsLogger = Factory.MsLogger();
        AzureOpenAIOptions = Factory.Services.GetRequiredService<IOptions<Configuration.AzureOpenAIOptions>>().Value;
        BingOptions = Factory.Services.GetRequiredService<IOptions<BingOptions>>().Value;
        OpenAIOptions = Factory.Services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        TestFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        QdrantOptions = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        LlamaModelOptions = Factory.Services.GetRequiredService<IOptions<LlamaModelOptions>>().Value;
        RequestFactory = Factory.Services.GetRequiredService<IModelRequestFactory>();
        LlamaModelFactory = Factory.Services.GetRequiredService<ILlamaModelFactory>();
    }


}
