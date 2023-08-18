using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using Embeddings.Qdrant.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

using TiktokenSharp;

using Xunit.Abstractions;

namespace Embeddings.Qdrant.Tests;

[Collection("EmbeddingsAndVectorDb Collection")]
public class TestOfTikTokenEmbeddings
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly string testFilesPath;
    private readonly IServiceProvider services;

    public TestOfTikTokenEmbeddings(EmbeddingsVectorDbTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.BuildFactoryWithLogging(output);
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
    }

    private const string CollectionName = "tiktoken-embeddings-test-collection";

    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(CollectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Collection {collectionName} deleted", CollectionName),
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task RunTikToken()
    {
        TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
        var i = tikToken.Encode("hello world"); //[15339, 1917]
        var d = tikToken.Decode(i); //hello world
        logger.Information($"TikToken: {string.Join(' ', i.Select(x => x))}");
        logger.Information($"TikToken: {d}");

        //use encoding name
        tikToken = TikToken.GetEncoding("cl100k_base");
        i = tikToken.Encode("hello world"); //[15339, 1917]
        d = tikToken.Decode(i); //hello world 
        logger.Information($"TikToken: {string.Join(' ', i.Select(x => x))}");
        logger.Information($"TikToken: {d}");
    }

    [Fact]
    public async Task RunTikTokenOnUberFiles()
    {
        var files = CreateUberFiles();
    }

    private PointCollection CreateUberFiles()
    {
        TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
        var files = new List<UberFile>();
        var allFiles = Directory.GetFiles(testFilesPath, "*.html", SearchOption.AllDirectories);
        foreach (var file in allFiles)
        {
            var content = File.ReadAllText(file);
            files.Add(new UberFile()
            {
                Id = Guid.NewGuid(),
                Content = content,
                Description = file,
                Tokens = tikToken.Encode(content)
            });
        }

        //TODO: create the Points
        //var modelParams = llamaModelFactory.CreateModelParams();
        //var embedder = llamaModelFactory.CreateEmbedder(modelParams);

        var points = new PointCollection();
        //foreach (var document in documents)
        //{
        //    var point = new PointStruct()
        //    {
        //        Id = Guid.NewGuid(),
        //        Payload = new Dictionary<string, string>()
        //        {
        //            {"name", document.Name},
        //            {"author", document.Author},
        //            {"year", document.Year.ToString()},
        //            {"description", document.Description},
        //        },
        //        Vector = embedder.GetEmbeddings(document.Description).Select(x => (double)x).ToArray()
        //    };
        //    points.Add(point);
        //}

        return points;
    }



    public class UberFile
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public IList<int> Tokens { get; set; }
    }

}
