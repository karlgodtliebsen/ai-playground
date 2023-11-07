using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using Kernel.Memory.NewsFeed.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticMemory;
using OpenAI.Client.Configuration;
using Xunit.Abstractions;

namespace Kernel.Memory.NewsFeed.Tests;

[Collection("Semantic Memory Collection")]
public class TestOfWikiMediaFeedWithSemanticMemory : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIConfiguration openAIConfiguration;
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
            //.WithQdrantSupport()
            .Build();

        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.memory = services.GetRequiredService<ISemanticMemoryClient>();
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
