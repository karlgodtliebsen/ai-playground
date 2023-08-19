using System.Text.Json.Serialization;

using AI.Library.Utils;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using Embeddings.Qdrant.Tests.Fixtures;

using FluentAssertions;

using LLama;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;


namespace Embeddings.Qdrant.Tests;

/// <summary>
/// <a href="https://qdrant.tech/documentation/tutorials/search-beginners">Search for beginners</a>
/// </summary>
[Collection("EmbeddingsAndVectorDb Collection")]
public class TestOfSearchScenarioInVectorDbUsingEmbeddings
{
    private readonly ILogger logger;

    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private readonly ILlamaModelFactory llamaModelFactory;
    private string modelFilesPath;
    private readonly IServiceProvider services;
    private const string CollectionName = "books-search-collection";

    private const int VectorSize = 3; // vey small vector size for testing

    public TestOfSearchScenarioInVectorDbUsingEmbeddings(EmbeddingsVectorDbTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.BuildFactoryWithLogging(output).WithDockerSupport();
        this.services = hostApplicationFactory.Services;
        var options = services.GetRequiredService<IOptions<LlamaModelOptions>>().Value;
        this.modelFilesPath = Path.GetFullPath(options.ModelPath);
        this.llamaModelFactory = services.GetRequiredService<ILlamaModelFactory>();
        this.logger = services.GetRequiredService<ILogger>();
    }

    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(CollectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information("{collectionName} deleted", CollectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    private async Task CreateCollectionInVectorDb(int size)
    {
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(size, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, cancellationToken: CancellationToken.None);

        var result = await client.CreateCollection(CollectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    //NOTE: uses all the downloaded models. This is very time consuming, so...
    //[Theory]
    [Theory(Skip = "Very time consuming. Download the models before running this test")]
    [InlineData("wizardLM-7B.ggmlv3.q4_1.bin")]
    [InlineData("ggml-vic13b-uncensored-q4_1.bin")]
    [InlineData("ggml-vic13b-uncensored-q5_0.bin")]
    [InlineData("ggml-vicuna-13B-1.1-q4_0.bin")]
    [InlineData("ggml-vicuna-13B-1.1-q8_0.bin")]
    [InlineData("wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin")]
    public async Task UseDifferentModels(string model)
    {
        //fi to point at folder for all the models, outside of this solution
        modelFilesPath = Path.Combine(Path.GetDirectoryName(modelFilesPath), model);
        var modelParams = llamaModelFactory.CreateModelParams();
        modelParams.ModelPath = modelFilesPath;
        var embedder = llamaModelFactory.CreateEmbedder(modelParams);
        await RunEmbedding(embedder);
    }


    [Fact]
    public async Task AddEmbeddingVectorToCollectionAndUseSearchToFindBook()
    {
        var modelParams = llamaModelFactory.CreateModelParams();
        var embedder = llamaModelFactory.CreateEmbedder(modelParams);
        await RunEmbedding(embedder);
    }

    private async Task RunEmbedding(LLamaEmbedder embedder)
    {
        await CleanupCollection();
        var points = CreatePointsusingDocuments();
        int numberOfDimensions = points.Dimension;
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(numberOfDimensions, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, cancellationToken: CancellationToken.None);

        await CreateCollectionInVectorDb(numberOfDimensions);
        points.ToJson(serializerOptions);

        var result = await client.Upsert(CollectionName, points, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );

        var vector = embedder
            .GetEmbeddings("alien invasion")
            .Select(x => (double)x).ToArray();

        //query_vector=encoder.encode("alien invasion").tolist(),

        var search = new SearchRequest()
            .Take(3)
            .SimilarToVector(vector)
            .UseWithPayload(true);

        var searchResult = await client.Search(CollectionName, search, CancellationToken.None);
        searchResult.Switch(

            res =>
            {
                res.Length.Should().Be(search.Limit);
                foreach (var r in res)
                {
                    logger.Information("Result: {payload} {score} {id}", r.Payload.ToJson(), r.Score, r.Id);
                }
            },
            error => throw new QdrantException(error.Error)
        );
    }


    /// <summary>
    /// <a href="https://qdrant.tech/documentation/tutorials/search-beginners">Search for beginners</a>
    /// </summary>
    /// <returns></returns>
    private PointCollection CreatePointsusingDocuments()
    {

        var documents = new List<Document>()
        {
            new Document { Name = "The Time Machine", Description = "A man travels through time and witnesses the evolution of humanity.", Author = "H.G. Wells", Year = 1895 },
            new Document { Name = "Ender's Game", Description = "A young boy is trained to become a military leader in a war against an alien race.", Author = "Orson Scott Card", Year = 1985 },
            new Document { Name = "Brave New World", Description = "A dystopian society where people are genetically engineered and conditioned to conform to a strict social hierarchy.", Author = "Aldous Huxley", Year = 1932 },
            new Document { Name = "The Hitchhiker's Guide to the Galaxy", Description = "A comedic science fiction series following the misadventures of an unwitting human and his alien friend.", Author = "Douglas Adams", Year = 1979 },
            new Document { Name = "Dune", Description = "A desert planet is the site of political intrigue and power struggles.", Author = "Frank Herbert", Year = 1965 },
            new Document { Name = "Foundation", Description = "A mathematician develops a science to predict the future of humanity and works to save civilization from collapse.", Author = "Isaac Asimov", Year = 1951 },
            new Document { Name = "Snow Crash", Description = "A futuristic world where the internet has evolved into a virtual reality metaverse.", Author = "Neal Stephenson", Year = 1992 },
            new Document { Name = "Neuromancer", Description = "A hacker is hired to pull off a near-impossible hack and gets pulled into a web of intrigue.", Author = "William Gibson", Year = 1984 },
            new Document { Name = "The War of the Worlds", Description = "A Martian invasion of Earth throws humanity into chaos.", Author = "H.G. Wells", Year = 1898 },
            new Document { Name = "The Hunger Games", Description = "A dystopian society where teenagers are forced to fight to the death in a televised spectacle.", Author = "Suzanne Collins", Year = 2008 },
            new Document { Name = "The Andromeda Strain", Description = "A deadly virus from outer space threatens to wipe out humanity.", Author = "Michael Crichton", Year = 1969 },
            new Document { Name = "The Left Hand of Darkness", Description = "A human ambassador is sent to a planet where the inhabitants are genderless and can change gender at will.", Author = "Ursula K. Le Guin", Year = 1969 },
            new Document { Name = "The Three-Body Problem", Description = "Humans encounter an alien civilization that lives in a dying system.", Author = "Liu Cixin", Year = 2008 }
        };
        var modelParams = llamaModelFactory.CreateModelParams();
        var embedder = llamaModelFactory.CreateEmbedder(modelParams);

        var points = new PointCollection();
        foreach (var document in documents)
        {
            var point = new PointStruct()
            {
                Id = Guid.NewGuid().ToString(),
                Payload = new Dictionary<string, string>()
                {
                    {"name", document.Name},
                    {"author", document.Author},
                    {"year", document.Year.ToString()},
                    {"description", document.Description},
                },
                Vector = embedder.GetEmbeddings(document.Description).Select(x => (double)x).ToArray()
            };
            points.Add(point);
        }

        return points;
    }



    public class Document
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
    }
}

