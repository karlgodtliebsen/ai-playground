using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;

using Xunit.Abstractions;

namespace Embeddings.Qdrant.Tests.Fixtures;

public sealed class EmbeddingsVectorDbTestFixture : IDisposable
{
    public ILogger Logger { get; private set; }
    public HostApplicationFactory Factory { get; private set; }
    public QdrantOptions QdrantOptions { get; private set; }
    public OpenAIOptions OpenAIOptions { get; private set; }
    public LlamaModelOptions LlamaModelOptions { get; private set; }

    public string TestFilesPath { get; private set; }
    public string ModelFilesPath { get; private set; }

    private readonly Func<ITestOutputHelper>? getOutput;
    public ITestOutputHelper Output { get; set; }

    public IModelRequestFactory RequestFactory { get; private set; }
    public ILlamaModelFactory LlamaModelFactory { get; private set; }
    public TestContainerDockerLauncher Launcher { get; private set; }
    public Func<bool> Launch { get; set; }

    public EmbeddingsVectorDbTestFixture()
    {
        getOutput = () => Output!;
        Factory = HostApplicationFactory.Build(
             environment: () => "IntegrationTests",
             serviceContext: (services, configuration) =>
             {
                 services
                     .AddQdrant(configuration)
                     .AddOpenAIConfiguration(configuration)
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
        Logger = Factory.Services.GetRequiredService<ILogger>();
        QdrantOptions = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        LlamaModelOptions = Factory.Services.GetRequiredService<IOptions<LlamaModelOptions>>().Value;
        OpenAIOptions = Factory.Services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        TestFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        ModelFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LlamaModelOptions.ModelPath);
        RequestFactory = Factory.Services.GetRequiredService<IModelRequestFactory>();
        LlamaModelFactory = Factory.Services.GetRequiredService<ILlamaModelFactory>();
        Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
        Launcher.Start();
    }


    public void Dispose()
    {
        Launcher.Stop();
    }
}
