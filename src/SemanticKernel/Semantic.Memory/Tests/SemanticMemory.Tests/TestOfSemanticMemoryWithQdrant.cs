using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticMemory;

using OpenAI.Client.Configuration;

using SemanticMemory.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticMemory.Tests;

[Collection("Semantic Memory Collection")]
public class TestOfSemanticMemoryWithQdrant : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticMemoryTestFixture fixture;
    private static readonly string FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

    private readonly ISemanticMemoryClient memory;
    const string NasaQuestion = "Any news from NASA about Orion?";

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    public TestOfSemanticMemoryWithQdrant(SemanticMemoryTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture
            .WithOutputLogSupport<TestFixtureBaseWithDocker>(output)
            .WithQdrantSupport()
            .Build();

        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;

        memory = new MemoryClientBuilder()
            .WithOpenAIDefaults(openAIOptions.ApiKey)
            .Build();
    }

    private string CreateNasaFile()
    {
        var file = Path.Combine(FullPath, "Documents\\NASA-news.pdf");
        if (!File.Exists(file))
        {
            throw new FileNotFoundException(file);
        }
        return file;
    }

    [Fact]
    public async Task RunNasaMemorySampleWithQdrantSupport()
    {

        //TODO: introduce qdrant support

        var file = CreateNasaFile();

        await memory.ImportDocumentAsync(file);
        var answer = await memory.AskAsync(NasaQuestion);

        logger.Information(answer.Result + "/n");

        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }
}
