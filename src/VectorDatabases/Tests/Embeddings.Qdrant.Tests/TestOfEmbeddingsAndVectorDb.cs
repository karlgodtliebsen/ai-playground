﻿using System.Text.Json.Serialization;

using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using Embeddings.Qdrant.Tests.Fixtures;

using FluentAssertions;

using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.Requests;

using Xunit.Abstractions;

namespace Embeddings.Qdrant.Tests;

[Collection("EmbeddingsAndVectorDb Collection")]
public class TestOfEmbeddingsAndVectorDb
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly QdrantOptions qdrantOptions;
    private readonly OpenAIOptions openAIOptions;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly EmbeddingsVectorDbTestFixture fixture;
    private readonly IModelRequestFactory requestFactory;
    private readonly string testFilesPath;

    public TestOfEmbeddingsAndVectorDb(EmbeddingsVectorDbTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.output = output;
        this.hostApplicationFactory = fixture.Factory;
        this.requestFactory = fixture.RequestFactory;
        this.fixture = fixture;
        this.qdrantOptions = fixture.QdrantOptions;
        this.openAIOptions = fixture.OpenAIOptions;
        this.testFilesPath = fixture.TestFilesPath;

        this.logger = fixture.Logger;
        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private const string collectionName = "embeddings-test-collection";

    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine($"Collection {collectionName} deleted"),
            error => throw new QdrantException(error.Error)
        );
    }

    //https://learn.microsoft.com/en-us/semantic-kernel/memories/

    /*
    So basically you take a sentence, paragraph, or entire page of text, 
    and then generate the corresponding embedding vector. And when a query is performed, 
    the query is transformed to its embedding representation, and then a search is performed through
    all the existing embedding vectors to find the most similar ones. 
    This is similar to when you make a search query on Bing, 
    and it gives you multiple results that are proximate to your query. 
    Semantic memory is not likely to give you an exact match — but it will always give you a set 
    of matches ranked in terms of how similar your query matches other pieces of text.
    */


    [Fact(Skip = "Takes to long. Needs solving")]
    public async Task VerifyEmbeddingsModelLlaMa()
    {
        var service = hostApplicationFactory.Services.GetRequiredService<IEmbeddingsService>();

        var fileData = await File.ReadAllTextAsync(Path.Combine(testFilesPath, "UBER_2019.html"));
        var f = fileData.Substring(0, fileData.Length / 2000);

        var message = new EmbeddingsMessage(f)
        {
            UserId = "user-42",
        };

        var result = await service.GetEmbeddings(message, CancellationToken.None);
        result.Length.Should().BeGreaterThan(1);

        await VerifyCreateCollectionInVectorDb(result.Length);
        var vector = result.Select(e => (double)e).ToArray();
        await AddDataToCollection(42, vector);
    }


    [Fact(Skip = "To many request. Needs solving")]
    // [Fact]
    public async Task VerifyEmbeddingsModelUsingOpenAIClient()
    {
        var fileData = await File.ReadAllTextAsync(Path.Combine(testFilesPath, "UBER_2019.html"));

        //https://platform.openai.com/docs/models/overview
        var aiClient = hostApplicationFactory.Services.GetRequiredService<IEmbeddingsAIClient>();
        var payload = requestFactory.CreateRequest<EmbeddingsRequest>(() =>
            new EmbeddingsRequest
            {
                Model = "text-embedding-ada-002",
                Input = fileData,
                //Input = "The food was delicious and the waiter...",
                User = "the user",
            });

        var response = await aiClient.GetEmbeddingsAsync(payload, CancellationToken.None);
        var data = response.Match(
            embeddings =>
            {
                embeddings.Data.Count.Should().Be(1);
                var data = embeddings.Data[0];
                //data.Embedding.Length.Should().Be(1536);
                output.WriteLine(data.Embedding.Length.ToString());
                return data;

            },
        error => throw new AIException(error.Error)
        );
        await VerifyCreateCollectionInVectorDb(data.Embedding.Length);
        await AddDataToCollection(data.Index, data.Embedding);
    }


    private async Task VerifyCreateCollectionInVectorDb(int size)
    {
        await CleanupCollection();
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var vectorParams = client.CreateParams(size, Distance.DOT, false);
        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine($"Succeeded creating Collection: {collectionName}"),
            error => throw new QdrantException(error.Error)
        );
    }

    private async Task AddDataToCollection(long index, double[] embeddings)
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var points = CreatePoints(index, embeddings);
        var result = await client.Upsert(collectionName, points, CancellationToken.None);
        result.Switch(

            _ => output.WriteLine("Succeeded adding vector to qdrant: "),
            error => throw new QdrantException(error.Error)
        );
    }

    //https://qdrant.github.io/qdrant/redoc/index.html#tag/points/operation/search_points
    private PointCollection CreatePoints(long index, double[] embeddings)
    {
        var points = new PointCollection()
        {
            new PointStruct()
            {
                //Id = "1",
                //Select(e => (float)e).ToArray()
                //VectorOfArray = embeddings,
                //Payload = new Dictionary<string, object>()
                //{
                //    { "something", "text" }
                //}
            }
        };
        return points;
    }
}