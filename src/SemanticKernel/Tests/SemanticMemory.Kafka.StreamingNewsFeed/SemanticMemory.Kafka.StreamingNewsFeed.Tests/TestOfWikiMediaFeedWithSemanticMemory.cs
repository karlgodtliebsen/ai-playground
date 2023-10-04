using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticMemory;

using OpenAI.Client.Configuration;

using SemanticMemory.Kafka.StreamingNewsFeed.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Tests;

[Collection("Semantic Memory Collection")]
public class TestOfWikiMediaFeedWithSemanticMemory : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticMemoryTestFixture fixture;
    private readonly ISemanticMemoryClient memory;

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    public TestOfWikiMediaFeedWithSemanticMemory(SemanticMemoryTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture
            .WithOutputLogSupport<TestFixtureBaseWithDocker>(output)
            //.WithKafkaSupport()
            .WithQdrantSupport()
            .Build();

        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;

        memory = new MemoryClientBuilder()
            .WithOpenAIDefaults(openAIOptions.ApiKey)
            .Build();
    }


    [Fact]
    public async Task RunWikiMediaStreamWithIndexSample()
    {
        //var file = CreateNasaFile();

        //var docId = await memory.ImportDocumentAsync(file, index: "index001");
        //var answer = await memory.AskAsync(NasaQuestion, index: "index001");

        //logger.Information(answer.Result + "/n");

        //foreach (var x in answer.RelevantSources)
        //{
        //    logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        //}
    }


}
