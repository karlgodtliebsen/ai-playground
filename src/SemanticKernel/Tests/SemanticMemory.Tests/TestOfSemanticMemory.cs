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
public class TestOfSemanticMemory : IAsyncLifetime
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

    public TestOfSemanticMemory(SemanticMemoryTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
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

    //[Fact]
    //public async Task RunSimpleMemorySample()
    //{

    //    // Import a file
    //    await memory.ImportDocumentAsync("Documents\\meeting-transcript.docx", tags: new() { { "user", "Blake" } });

    //    // Import multiple files and apply multiple tags
    //    await memory.ImportDocumentAsync(new Document("file001")
    //        .AddFile("business-plan.docx")
    //        .AddFile("project-timeline.pdf")
    //        .AddTag("user", "Blake")
    //        .AddTag("collection", "business")
    //        .AddTag("collection", "plans")
    //        .AddTag("fiscalYear", "2023"));

    //    var answer1 = await memory.AskAsync("How many people attended the meeting?");
    //    //var answer2 = await memory.AskAsync("what's the project timeline?", filter: new MemoryFilter().ByTag("user", "Blake"));

    //    //Use qdrant
    //}

    [Fact]
    public async Task RunNasaMemorySample()
    {
        var file = CreateNasaFile();
        //var file = "NASA-news.pdf";

        await memory.ImportDocumentAsync(file);
        var answer = await memory.AskAsync(NasaQuestion);

        logger.Information(answer.Result + "/n");

        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }

    [Fact]
    public async Task RunNasaMemoryWithIndexSample()
    {
        var file = CreateNasaFile();

        var docId = await memory.ImportDocumentAsync(file, index: "index001");
        var answer = await memory.AskAsync(NasaQuestion, index: "index001");

        logger.Information(answer.Result + "/n");

        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }

    [Fact]
    public async Task RunNasaMemoryWithTagSample()
    {
        var file = CreateNasaFile();

        var docId = await memory.ImportDocumentAsync(new Document().AddFile(file).AddTag("user", "USER-333"));

        // OK
        var answer = await memory.AskAsync(NasaQuestion);
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }

        // OK
        answer = await memory.AskAsync(NasaQuestion, MemoryFilters.ByTag("user", "USER-333"));

        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }


    [Fact]
    public async Task RunNasaMemoryWithindexAndTagSample()
    {
        var file = CreateNasaFile();

        // Upload a document in specific user and tag with user ID.
        var docId = await memory.ImportDocumentAsync(new Document().AddFile(file).AddTag("user", "USER-333"), index: "index002");

        // NO ANSWER: the data is not in the default index
        var answer = await memory.AskAsync(NasaQuestion);
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
        // NO ANSWER: even if the filter is correct, the data is not in the default index
        answer = await memory.AskAsync(NasaQuestion, MemoryFilters.ByTag("user", "USER-333"));
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
        // OK
        answer = await memory.AskAsync(NasaQuestion, index: "index002", MemoryFilters.ByTag("user", "USER-333"));

        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }

        // IMPORTANT: this command is missing the user tag and the service will return the data.
        //            This is equivalent to an admin having full access.
        answer = await memory.AskAsync(NasaQuestion, index: "index002");
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }

}
