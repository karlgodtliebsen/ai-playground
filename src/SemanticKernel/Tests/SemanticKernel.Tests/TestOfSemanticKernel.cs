using System.Text.Json.Serialization;

using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;
using AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

using FluentAssertions;

using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Text;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

//using Azure.Core;
//using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;


namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernel
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;

    private readonly HostApplicationFactory hostApplicationFactory;

    private readonly AzureOpenAIOptions azureOpenAIOptions;
    private readonly OpenAIOptions openAIOptions;
    private readonly QdrantOptions qdrantOptions;
    private readonly JsonSerializerOptions serializerOptions;
    private readonly SemanticKernelTestFixture fixture;
    private readonly string testFilesPath;
    private readonly ILlamaModelFactory llamaModelFactory;

    public TestOfSemanticKernel(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;

        this.hostApplicationFactory = fixture.Factory;
        this.azureOpenAIOptions = fixture.AzureOpenAIOptions;
        this.openAIOptions = fixture.OpenAIOptions;
        this.testFilesPath = fixture.TestFilesPath;
        this.qdrantOptions = fixture.QdrantOptions;
        this.llamaModelFactory = fixture.LlamaModelFactory;

        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private const string collectionName = "SemanticKernel-test-collection";

    private async Task CleanupCollection()
    {
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();
        var result = await client.RemoveCollection(collectionName, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Collection {collectionName} deleted", collectionName),
            error => throw new QdrantException(error.Error)
        );
    }

    private async Task CleanUpAndCreateCollectionInVectorDb(int size)
    {
        await CleanupCollection();

        var qdrantFactory = hostApplicationFactory.Services.GetRequiredService<IQdrantFactory>();
        var vectorParams = qdrantFactory.CreateParams(size, Distance.COSINE, true);
        var client = await qdrantFactory.Create(collectionName, vectorParams, cancellationToken: CancellationToken.None);

        var result = await client.CreateCollection(collectionName, vectorParams, CancellationToken.None);
        result.Switch(

            _ => logger.Information("Succeeded Creating Collection{collectionName}", collectionName),
            error => throw new QdrantException(error.Error)
        );
    }


    [Fact]
    public async Task UseQdrantMemoryCollectionc()
    {
        var url = this.qdrantOptions.Url;
        var completionModel = "text-davinci-003";
        var embeddingModel = "text-embedding-ada-002";
        const int openAiVectorSize = 1536;
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        var memoryStorage = await factory.Create(collectionName, openAiVectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = Kernel.Builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService(embeddingModel, openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();

        logger.Information("== Printing Collections in DB ==");
        var collections = memoryStorage.GetCollectionsAsync();
        await foreach (var collection in collections)
        {
            logger.Information(collection);
        }

        logger.Information("== Adding Memories ==");

        var key1 = await kernel.Memory.SaveInformationAsync(collectionName, id: "cat1", text: "british short hair");
        var key2 = await kernel.Memory.SaveInformationAsync(collectionName, id: "cat2", text: "orange tabby");
        var key3 = await kernel.Memory.SaveInformationAsync(collectionName, id: "cat3", text: "norwegian forest cat");

        logger.Information("== Printing Collections in DB ==");
        collections = memoryStorage.GetCollectionsAsync();
        await foreach (var collection in collections)
        {
            logger.Information(collection);
        }

        logger.Information("== Retrieving Memories Through the Kernel ==");
        MemoryQueryResult? lookup = await kernel.Memory.GetAsync(collectionName, "cat1");
        logger.Information(lookup is not null ? lookup.Metadata.Text : "ERROR: memory not found");
        lookup.Should().NotBeNull();
        lookup!.Metadata.Should().NotBeNull();
        lookup!.Metadata.Text.Should().NotBeNull();
        lookup!.Metadata.Text.Should().Be("british short hair");

        logger.Information("== Retrieving Memories Directly From the Store ==");
        var memory1 = await memoryStorage.GetWithPointIdAsync(collectionName, key1);
        var memory2 = await memoryStorage.GetWithPointIdAsync(collectionName, key2);
        var memory3 = await memoryStorage.GetWithPointIdAsync(collectionName, key3);
        logger.Information(memory1 != null ? memory1.Metadata.Text : "ERROR: memory not found");
        logger.Information(memory2 != null ? memory2.Metadata.Text : "ERROR: memory not found");
        logger.Information(memory3 != null ? memory3.Metadata.Text : "ERROR: memory not found");
        memory1.Should().NotBeNull();
        memory2.Should().NotBeNull();
        memory3.Should().NotBeNull();


        logger.Information("== Similarity Searching Memories: My favorite color is orange ==");
        var searchResults = kernel.Memory.SearchAsync(collectionName, "My favorite color is orange", limit: 3, minRelevanceScore: 0.8);

        await foreach (var item in searchResults)
        {
            logger.Information(item.Metadata.Text + " : " + item.Relevance);
        }

        //logger.Information("== Removing Collection {0} ==", collectionName);
        //await memoryStorage.DeleteCollectionAsync(collectionName);

        //logger.Information("== Printing Collections in DB ==");
        //await foreach (var collection in collections)
        //{
        //    logger.Information(collection);
        //}
    }

    [Fact]
    public async Task RunSimpleSummarizationSample()
    {
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md

        var builder = new KernelBuilder();
        builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextCompletionService("text-davinci-003", openAIOptions.ApiKey);
        var kernel = builder.Build();
        var prompt = @"{{$input}}

One line TLDR with the fewest words.";

        var summarize = kernel.CreateSemanticFunction(prompt);

        string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        string text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";

        var r1 = await summarize.InvokeAsync(text1);
        var r2 = await summarize.InvokeAsync(text2);

        logger.Information(r1.ToString());
        logger.Information(r2.ToString());
    }

    [Fact]
    public async Task RunChainedSample()
    {
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md

        var builder = new KernelBuilder();
        builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextCompletionService("text-davinci-003", openAIOptions.ApiKey);

        var kernel = builder.Build();
        string translationPrompt = @"{{$input}}

Translate the text to math.";

        string summarizePrompt = @"{{$input}}

Give me a TLDR with the fewest words.";

        var translator = kernel.CreateSemanticFunction(translationPrompt);
        var summarize = kernel.CreateSemanticFunction(summarizePrompt);

        string inputText = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        // Run two prompts in sequence (prompt chaining)
        var output = await kernel.RunAsync(inputText, translator, summarize);
        logger.Information(output.ToString());
    }


    [Fact(Skip = "TODO: Get the uri for Azure Cognitive Search")]
    public async Task RunSampleUsingAzureCognitiveSearch()
    {
        var kernelWithACS = Kernel.Builder
            .WithLogger(fixture.MsLogger)
            .WithAzureCognitiveSearchMemory("", openAIOptions.ApiKey)
            .Build();
    }

    [Fact]
    public async Task RunTextSampleUsingSemanticKernel()
    {
        //https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/KernelSyntaxExamples
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs

        //var memoryStorage = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStore>();
        var completionModel = "text-davinci-003";
        var embeddingModel = "text-embedding-ada-002";
        const int openAiVectorSize = 1536;
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        var memoryStorage = await factory.Create(collectionName, openAiVectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = Kernel.Builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextCompletionService(completionModel, openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService(embeddingModel, openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();
        string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

        await kernel.Memory.SaveReferenceAsync(
            collection: collectionName,
            //description: text1,
            text: text1,
            externalId: Guid.NewGuid().ToString(),
            externalSourceName: "Uber"
        );
        logger.Information("saved.");

    }
    [Fact(Skip = "To many requests error")]
    public async Task RunUberFilesSampleUsingSemanticKernel()
    {
        var files = LoadUberFiles(CancellationToken.None);
        //https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples/KernelSyntaxExamples
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs


        var memoryStorage = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStore>();

        IKernel kernel = Kernel.Builder
            .WithLogger(fixture.MsLogger)
            .WithOpenAITextCompletionService("text-davinci-003", openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002", openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();

        int i = 0;
        await foreach (var file in files)
        {
            await kernel.Memory.SaveReferenceAsync(
                collection: collectionName,
                description: file.Content,
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
        var client = hostApplicationFactory.Services.GetRequiredService<IQdrantVectorDb>();

        await CleanUpAndCreateCollectionInVectorDb(1536);
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
        var files = new List<UberFile>();
        var allFiles = Directory.GetFiles(testFilesPath, "*.html", SearchOption.AllDirectories);

        var builder = new KernelBuilder();
        builder.WithOpenAITextCompletionService("text-davinci-003", openAIOptions.ApiKey);
        var kernel = builder.Build();
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
