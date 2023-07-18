﻿using System.Text.Json.Serialization;

using AI.Library.Utils;
using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

using Embeddings.Qdrant.Tests.Fixtures;

using FluentAssertions;

using LLama;

using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Domain;

using Xunit.Abstractions;


namespace Embeddings.Qdrant.Tests;

/// <summary>
/// <a href="https://qdrant.tech/documentation/tutorials/search-beginners">Search for beginners</a>
/// </summary>
[Collection("EmbeddingsAndVectorDb Collection")]
public class TestOfSearchScenarioInVectorDbUsingEmbeddings
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;

    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly EmbeddingsVectorDbTestFixture fixture;

    private readonly ILlamaModelFactory llamaModelFactory;
    private string modelFilesPath;

    //private LLamaEmbedder embedder;
    private readonly QdrantOptions qdrantOptions;
    private readonly IModelRequestFactory requestFactory;
    private readonly string testFilesPath;


    public TestOfSearchScenarioInVectorDbUsingEmbeddings(EmbeddingsVectorDbTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.output = output;
        this.logger = fixture.Logger;
        this.hostApplicationFactory = fixture.Factory;
        this.requestFactory = fixture.RequestFactory;
        this.fixture = fixture;
        this.qdrantOptions = fixture.QdrantOptions;
        this.testFilesPath = fixture.TestFilesPath;
        this.modelFilesPath = fixture.ModelFilesPath;
        this.llamaModelFactory = fixture.LlamaModelFactory;
        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private const string collectionName = "books-search-collection";

    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information("{collectionName} deleted", collectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    private async Task CreateCollectionInVectorDb(int size)
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var vectorParams = client.CreateParams(size, Distance.COSINE, true);
        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded"),
            error => throw new QdrantException(error.Error)
        );
    }

    //NOTE: uses all the downloaded models. This is very time consuming, so...

    [Theory(Skip = "Very time consuming. Download the models before running this test")]
    [InlineData("LlamaModels/wizardLM-7B.ggmlv3.q4_1.bin")]
    [InlineData("LlamaModels/ggml-vic13b-uncensored-q4_1.bin")]
    [InlineData("LlamaModels/ggml-vic13b-uncensored-q5_0.bin")]
    [InlineData("LlamaModels/ggml-vicuna-13B-1.1-q4_0.bin")]
    [InlineData("LlamaModels/ggml-vicuna-13B-1.1-q8_0.bin")]
    [InlineData("LlamaModels/wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin")]
    public async Task UseDifferentModels(string model)
    {
        modelFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, model);
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
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();

        var points = CreatePointsusingDocuments();
        int numberOfDimensions = points.Dimension;
        await CreateCollectionInVectorDb(numberOfDimensions);
        points.ToJson(serializerOptions);

        var result = await client.Upsert(collectionName, points, CancellationToken.None);
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

        var searchResult = await client.Search(collectionName, search, CancellationToken.None);
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
                Id = Guid.NewGuid().ToString("N"),
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
