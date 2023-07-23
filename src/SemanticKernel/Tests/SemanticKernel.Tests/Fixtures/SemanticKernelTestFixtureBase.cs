using AI.Test.Support;
using AI.Test.Support.DockerSupport;
using AI.VectorDatabase.Qdrant.Configuration;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;

using SemanticKernel.Tests.Configuration;

namespace SemanticKernel.Tests.Fixtures;

public class SemanticKernelTestFixtureBase : TestFixtureBase
{
    public AzureOpenAIOptions AzureOpenAIOptions { get; private set; }
    public OpenAIOptions OpenAIOptions { get; private set; }
    public QdrantOptions QdrantOptions { get; private set; }
    public BingOptions BingOptions { get; private set; }
    public HuggingFaceOptions HuggingFaceOptions { get; private set; }

    public LlamaModelOptions LlamaModelOptions { get; private set; }
    public IModelRequestFactory RequestFactory { get; private set; }
    public ILlamaModelFactory LlamaModelFactory { get; private set; }
    public string TestFilesPath { get; private set; }
    public string SkillsPath { get; private set; }

    public SemanticKernelTestFixtureBase()
    {
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
                    .AddHuggingFace(configuration)
                    .AddLlamaConfiguration(configuration)
                    .AddWebApiConfiguration(configuration)
                    ;
                services.AddSingleton<TestContainerDockerLauncher>();
                var section = configuration.GetSection(DockerLaunchOptions.SectionName);
                services.AddOptions<DockerLaunchOptions>().Bind(section);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        TestFilesPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));
        SkillsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills"));
        AzureOpenAIOptions = Factory.Services.GetRequiredService<IOptions<Configuration.AzureOpenAIOptions>>().Value;
        BingOptions = Factory.Services.GetRequiredService<IOptions<BingOptions>>().Value;
        OpenAIOptions = Factory.Services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        QdrantOptions = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
        HuggingFaceOptions = Factory.Services.GetRequiredService<IOptions<HuggingFaceOptions>>().Value;
        LlamaModelOptions = Factory.Services.GetRequiredService<IOptions<LlamaModelOptions>>().Value;
        RequestFactory = Factory.Services.GetRequiredService<IModelRequestFactory>();
        LlamaModelFactory = Factory.Services.GetRequiredService<ILlamaModelFactory>();
    }
}
