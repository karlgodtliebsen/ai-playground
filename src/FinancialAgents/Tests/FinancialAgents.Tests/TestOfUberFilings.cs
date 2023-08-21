using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;

using FinancialAgents.Tests.Fixtures;

using FluentAssertions;

using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace FinancialAgents.Tests;

[Collection("FinancialAgents Collection")]
public class TestOfUberFilings
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly string testFilesPath;
    private readonly ILlamaModelFactory llamaModelFactory;
    private readonly IEmbeddingsService embeddingService;
    private readonly IServiceProvider services;
    private const string CollectionName = "SemanticKernel-uber-test-collection";
    private const int VectorSize = 1536;

    public TestOfUberFilings(FinancialAgentsTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithOutputLogSupport(output).Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.llamaModelFactory = services.GetRequiredService<ILlamaModelFactory>();
        this.testFilesPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));
        this.embeddingService = hostApplicationFactory.Services.GetRequiredService<IEmbeddingsService>();
    }

    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(CollectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Collection {collectionName} deleted", CollectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    [Fact(Skip = "early work")]
    public async Task VerifyEmbeddingsModelLLama()
    {
        var files = LoadUberFiles(CancellationToken.None);
        await foreach (var uberFile in files)
        {
            var htmlContent = uberFile.Content;

            var parser = new HtmlParser();
            var document = parser.ParseDocument(htmlContent);       //can use stream for performance improvement
            foreach (IElement element in document.QuerySelectorAll("html > body > div"))
            {
                //logger.Information(element.TextContent);

                var message = new EmbeddingsMessage(element.TextContent)
                {
                    UserId = "user-42",
                };
                var result = await embeddingService.GetEmbeddings(message, CancellationToken.None);
                result.Length.Should().BeGreaterThan(1);
            }
            return;
        }

        //var fileData = await File.ReadAllTextAsync(Path.Combine(testFilesPath, "UBER_2019.html"));
        //var f = fileData.Substring(0, fileData.Length / 2000);

        //var message = new EmbeddingsMessage(f)
        //{
        //    UserId = "user-42",
        //};

        //var result = await embeddingService.GetEmbeddings(message, CancellationToken.None);
        //result.Length.Should().BeGreaterThan(1);

        //await VerifyCreateCollectionInVectorDb(result.Length);
        //var vector = result.Select(e => (double)e).ToArray();
        //await AddDataToCollection(42, vector);
    }

    private async IAsyncEnumerable<UberFile> LoadUberFiles(CancellationToken cancellationToken)
    {
        var files = new List<UberFile>();
        var allFiles = Directory.GetFiles(testFilesPath, "*.html", SearchOption.AllDirectories);

        foreach (var file in allFiles)
        {
            var year = Path.GetFileNameWithoutExtension(file).Split('_').Last();
            var content = await File.ReadAllTextAsync(file, cancellationToken);

            //we must clean up the content
            //var summarize = await summarizer.InvokeAsync(content, cancellationToken);
            var uberFile = new UberFile()
            {
                Id = Guid.NewGuid(),
                Content = content,
                //  Summary = summarize.Result,
                Name = file,
                // ChunkedContent = TextChunker.SplitPlainTextLines(content, 100),
                //Tokens = tikToken.Encode(content),
                Year = year
            };
            yield return uberFile;
        }
    }


    private async Task VerifyCreateCollectionInVectorDb(int size)
    {
        await CleanupCollection();
        //var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        //var vectorParams = client.CreateParams(size, Distance.DOT, false);

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(size, Distance.DOT, true);
        var client = await qdrantFactory.Create(CollectionName, vectorParams, cancellationToken: CancellationToken.None);

        var result = await client.CreateCollection(CollectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded creating Collection: {CollectionName}", CollectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    private async Task AddDataToCollection(long index, double[] embeddings)
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var points = CreatePoints(index, embeddings);
        var result = await client.Upsert(CollectionName, points, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded adding vector to qdrant: "),
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


    [Fact(Skip = "To many requests error")]
    //[Fact]
    public async Task RunUberFilesSampleUsingSemanticKernel()
    {
        var files = LoadUberFiles(CancellationToken.None);
        //https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/KernelSyntaxExamples
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs

        //var completionModel = "text-davinci-003";
        //var embeddingModel = "text-embedding-ada-002";
        //bool recreateCollection = true;
        //bool storeOnDisk = false;

        //var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        //var memoryStorage = await factory.Create(CollectionName, VectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        //IKernel kernel = Kernel.Builder
        //    .WithLogger(fixture.MsLogger)
        //    .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
        //    .WithOpenAITextEmbeddingGenerationService(embeddingModel, openAIOptions.ApiKey)
        //    .WithMemoryStorage(memoryStorage)
        //    .Build();

        //int i = 0;
        //await foreach (var file in files)
        //{
        //    await kernel.Memory.SaveReferenceAsync(
        //        collection: CollectionName,
        //        //description: file.Content,
        //        text: file.Summary,
        //        externalId: file.Id.ToString(),
        //        externalSourceName: "Uber"
        //    );
        //    logger.Information($" #{++i} saved.");
        //}
    }

    [Fact(Skip = "Performance")]
    public async Task RunUberFilesSampleUsingPointsLlama()
    {
        var vectorParams = new VectorParams(VectorSize, Distance.DOT, true);
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var client = await qdrantFactory.Create(CollectionName, vectorParams, recreateCollection: true, cancellationToken: CancellationToken.None);

        //    var files = LoadUberFiles(CancellationToken.None);
        //    var pointStructs = CreateUberFilesPoints(files);

        //    await foreach (var pointStruct in pointStructs)
        //    {
        //        var points = new List<PointStruct>() { pointStruct };
        //        var result = await client.Upsert(CollectionName, points, CancellationToken.None);
        //        result.Switch(
        //            _ => logger.Information("Succeeded"),
        //            error => throw new QdrantException(error.Error)
        //        );
        //    }
    }


    private async IAsyncEnumerable<PointStruct> CreateUberFilesPoints(IAsyncEnumerable<UberFile> files)
    {
        var modelParams = llamaModelFactory.CreateModelParams();
        var embedder = llamaModelFactory.CreateEmbedder(modelParams);

        await foreach (var file in files)
        {
            var payload = new Dictionary<string, string>()
            {
                {
                    "name", file.Name
                },
                {
                    "content", file.Content
                },
                {
                    "summary", file.Summary
                },
                {
                    "author", "Uber"
                },
                {
                    "year", file.Year
                }
            };
            var chunkVector = new List<double>();
            foreach (var chunk in file.ChunkedContent)
            {
                //double[] vector = embedder.GetEmbeddings(chunk).Select(x => (double)x).ToArray();
                //chunkVector.AddRange(vector);
            }
            var point = new PointStruct()
            {
                Id = file.Id.ToString(),
                Payload = payload,
                Vector = chunkVector.ToArray()
            };
            yield return point;
        }
    }

    private BatchRequestStruct CreateUberFilesBatch(IEnumerable<UberFile> files)
    {
        var modelParams = llamaModelFactory.CreateModelParams();
        var embedder = llamaModelFactory.CreateEmbedder(modelParams);
        var numberOfFiles = files.Count();
        var ids = new List<string>(numberOfFiles);
        var payloads = new List<Dictionary<string, object>>(numberOfFiles);
        double[][] vectors = new double[numberOfFiles][];

        int fileIndex = 0;

        foreach (var file in files)
        {
            ids.Add(file.Id.ToString());
            var payload = new Dictionary<string, object>()
            {
                {
                    "name", file.Name
                },
                {
                    "content", file.Content
                },
                {
                    "summary", file.Summary
                },
                {
                    "author", "Uber"
                },
                {
                    "year", file.Year.ToString()
                }
            };
            payloads.Add(payload);
            var chunkVector = new List<double>();
            foreach (var chunk in file.ChunkedContent)
            {
                double[] vector = embedder.GetEmbeddings(chunk).Select(x => (double)x).ToArray();
                chunkVector.AddRange(vector);
            }
            vectors[fileIndex++] = chunkVector.ToArray();
        }

        var batch = new BatchRequestStruct(vectors, ids, payloads);
        return batch;
    }

    public class UberFile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public string Year { get; set; }
        public List<string> ChunkedContent { get; set; }
        public IList<int> Tokens { get; set; }
    }

}
