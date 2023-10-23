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
        this.hostApplicationFactory = fixture
            .WithOutputLogSupport<TestFixtureBaseWithDocker>(output)
            .WithQdrantSupport()
            .Build();
        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;

        memory = new MemoryClientBuilder()
            .WithOpenAIDefaults(openAIOptions.ApiKey)
            //.WithCustomEmbeddingGeneration()
            //.WithCustomVectorDb()
            //.WithCustomTextGeneration
            .Build();
    }
    /*

    
    public MemoryClientBuilder WithCustomEmbeddingGeneration(
      ITextEmbeddingGeneration service,
      bool useForIngestion = true,
      bool useForRetrieval = true)
    {
      service = service ?? throw new ConfigurationException("The embedding generator instance is NULL");
      if (useForRetrieval)
        this.AddSingleton<ITextEmbeddingGeneration>(service);
      if (useForIngestion)
        this._embeddingGenerators.Add(service);
      return this;
    }

    public MemoryClientBuilder WithCustomVectorDb(
      ISemanticMemoryVectorDb service,
      bool useForIngestion = true,
      bool useForRetrieval = true)
    {
      service = service ?? throw new ConfigurationException("The vector DB instance is NULL");
      if (useForRetrieval)
        this.AddSingleton<ISemanticMemoryVectorDb>(service);
      if (useForIngestion)
        this._vectorDbs.Add(service);
      return this;
    }

    public MemoryClientBuilder WithCustomTextGeneration(ITextGeneration service)
    {
      service = service ?? throw new ConfigurationException("The text generator instance is NULL");
      this.AddSingleton<ITextGeneration>(service);
      return this;
    }
    */

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
        answer = await memory.AskAsync(NasaQuestion, filters: new List<MemoryFilter>() { MemoryFilters.ByTag("user", "USER-333") });

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
        answer = await memory.AskAsync(NasaQuestion, filters: new List<MemoryFilter>() { MemoryFilters.ByTag("user", "USER-333") });
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
    [Fact]
    public async Task RunNasaMemoryWithTwoUsersSample()
    {
        var file = CreateNasaFile();

        // Upload file, allow two users to access
        var docId = await memory.ImportDocumentAsync(new Document()
            .AddFile(file)
            .AddTag("user", "USER-333")
            .AddTag("user", "USER-444"));

        // OK: USER-333 tag matches
        var answer = await memory.AskAsync(NasaQuestion, filters: new List<MemoryFilter>() { MemoryFilters.ByTag("user", "USER-333") });
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }

        // OK: USER-444 tag matches
        answer = await memory.AskAsync(NasaQuestion, filters: new List<MemoryFilter>() { MemoryFilters.ByTag("user", "USER-444") });
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }

    [Fact]
    public async Task RunNasaMemoryWithCategorizedDataUsingTags()
    {
        var file = CreateNasaFile();

        // Upload file, allow two users to access, and add a content type tag for extra filtering
        var docId = await memory.ImportDocumentAsync(new Document()
            .AddFile(file)
            .AddTag("user", "USER-333")
            .AddTag("user", "USER-444")
            .AddTag("type", "planning"));

        // No information found, the type tag doesn't match
        var answer = await memory.AskAsync(NasaQuestion, filters: new List<MemoryFilter>() { MemoryFilters.ByTag("user", "USER-333"), MemoryFilters.ByTag("type", "email") });
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
        //Found using the planning tag
        answer = await memory.AskAsync(NasaQuestion, filters: new List<MemoryFilter>() { MemoryFilters.ByTag("user", "USER-333"), MemoryFilters.ByTag("type", "planning") });
        logger.Information(answer.Result + "/n");
        foreach (var x in answer.RelevantSources)
        {
            logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        }
    }

    [Fact]
    public async Task RunNasaMemoryWithCleanup()
    {
        var file = CreateNasaFile();
        string Id = System.IO.Path.GetFileName(file);

        await this.memory.ImportDocumentAsync(
            file,
            documentId: Id,
            steps: Constants.PipelineWithoutSummary);

        while (!await this.memory.IsDocumentReadyAsync(documentId: Id))
        {
            logger.Information("Waiting for memory ingestion to complete...");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        var answer = await this.memory.AskAsync("What is Orion?");
        logger.Information(answer.Result);
        Assert.Contains("spacecraft", answer.Result);

        logger.Information("Deleting memories extracted from the document");
        await this.memory.DeleteDocumentAsync(Id);
    }

}
