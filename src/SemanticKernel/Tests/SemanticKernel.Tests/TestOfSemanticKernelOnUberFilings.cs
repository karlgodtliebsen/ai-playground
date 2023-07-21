using AI.Test.Support;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Text;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Domain;
using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;


namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelOnUberFilings
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticKernelTestFixture fixture;
    private readonly string testFilesPath;
    private readonly ILlamaModelFactory llamaModelFactory;

    public TestOfSemanticKernelOnUberFilings(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.Factory;
        this.openAIOptions = fixture.OpenAIOptions;
        this.testFilesPath = fixture.TestFilesPath;
        this.llamaModelFactory = fixture.LlamaModelFactory;
    }

    private const string collectionName = "SemanticKernel-uber--test-collection";
    private const int vectorSize = 1536;

    [Fact(Skip = "To many requests error")]
    //[Fact]
    public async Task RunUberFilesSampleUsingSemanticKernel()
    {
        var files = LoadUberFiles(CancellationToken.None);
        //https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/KernelSyntaxExamples
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs

        var completionModel = "text-davinci-003";
        var embeddingModel = "text-embedding-ada-002";
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        var memoryStorage = await factory.Create(collectionName, vectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = Kernel.Builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService(embeddingModel, openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();

        int i = 0;
        await foreach (var file in files)
        {
            await kernel.Memory.SaveReferenceAsync(
                collection: collectionName,
                //description: file.Content,
                text: file.Summary,
                externalId: file.Id.ToString(),
                externalSourceName: "Uber"
            );
            logger.Information($" #{++i} saved.");
        }
    }

    [Fact(Skip = "Performance")]
    public async Task RunUberFilesSampleUsingPointsLlama()
    {
        var vectorParams = new VectorParams(vectorSize, Distance.DOT, true);
        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var client = await qdrantFactory.Create(collectionName, vectorParams, recreateCollection: true, cancellationToken: CancellationToken.None);

        var files = LoadUberFiles(CancellationToken.None);
        var pointStructs = CreateUberFilesPoints(files);

        await foreach (var pointStruct in pointStructs)
        {
            var points = new List<PointStruct>() { pointStruct };
            var result = await client.Upsert(collectionName, points, CancellationToken.None);
            result.Switch(
                _ => logger.Information("Succeeded"),
                error => throw new QdrantException(error.Error)
            );
        }
    }

    private async IAsyncEnumerable<UberFile> LoadUberFiles(CancellationToken cancellationToken)
    {
        //TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
        var completionModel = "text-davinci-003";

        var files = new List<UberFile>();
        var allFiles = Directory.GetFiles(testFilesPath, "*.html", SearchOption.AllDirectories);

        var kernel = new KernelBuilder()
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
            .Build();

        var prompt = @"{{$input}} One line TLDR with the fewest words.";
        var summarizer = kernel.CreateSemanticFunction(prompt);

        foreach (var file in allFiles)
        {
            var year = Path.GetFileNameWithoutExtension(file).Split('_').Last();
            var content = await File.ReadAllTextAsync(file, cancellationToken);

            //we must clean up the content
            var summarize = await summarizer.InvokeAsync(content, cancellationToken);
            var uberFile = new UberFile()
            {
                Id = Guid.NewGuid(),
                Content = content,
                Summary = summarize.Result,
                Name = file,
                ChunkedContent = TextChunker.SplitPlainTextLines(content, 100),
                //Tokens = tikToken.Encode(content),
                Year = year
            };
            yield return uberFile;
        }
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
