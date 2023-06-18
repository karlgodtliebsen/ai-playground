using AI.Domain.AIClients;
using AI.Domain.Configuration;
using AI.Domain.Models;
using AI.Domain.Tests.Utils;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Text.Json.Serialization;

using Xunit.Abstractions;

namespace AI.Domain.Tests;


public class TestOfOpenAIClients
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly OpenAIOptions options;
    private string path;
    public TestOfOpenAIClients(ITestOutputHelper output)
    {
        this.output = output;
        this.factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddAOpenAIConfiguration(configuration);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();
        options = factory.Services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        this.path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
    }


    [Fact]
    public async Task VerifyInitialAccessToTextCompletionModelForClient()
    {
        var aiClient = factory.Services.GetRequiredService<ICompletionAIClient>();


        //https://platform.openai.com/docs/models/overview
        //text-davinci-003
        //Can do any language task with better quality, longer output, and consistent instruction-following
        //than the curie, babbage, or ada models. Also supports some additional features such as inserting text.

        string deploymentName = "text-davinci-003"; //text-davinci-003 text-davinci-002
        string prompt = "Say this is a test";
        output.WriteLine($"Input: {prompt}");


        var payload = new CompletionRequest
        {
            Model = deploymentName,
            Prompt = prompt,
            MaxTokens = 7,
            Temperature = 0.0f,
            TopP = 1.0f,
            NumChoicesPerPrompt = 1,
            Stream = false,
            Logprobs = null,
            //Stop = "\n"
        };

        var completionsResponse = await aiClient.GetCompletionsAsync(payload, CancellationToken.None);
        completionsResponse.Should().NotBeNull();

        string completion = completionsResponse!.Value.Choices[0].Text.Trim();
        completion.Should().NotBeNullOrWhiteSpace();
        completion.Should().Be("This is indeed a test");
        output.WriteLine(completion);
    }

    [Fact]
    public async Task VerifyInitialAccessToTextChatCompletionModelForClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role = "system", Content = "You are a helpful assistant.!" },
            new ChatCompletionMessage { Role = "user", Content = "Hello!" }
        };

        var payload = new ChatCompletionRequest
        {
            Model = deploymentName,
            Messages = messages
        };

        var charCompletionsResponse = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
        charCompletionsResponse.Should().NotBeNull();

        string completion = charCompletionsResponse!.Value.Choices[0].Message.Content.Trim();
        completion.Should().NotBeNullOrWhiteSpace();
        completion.Should().Contain("Hello");
        completion.Should().Contain("I assist you today?");
        output.WriteLine(completion);
    }


    [Fact]
    public async Task VerifyInitialAccessToEditModelForClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IEditsAIClient>();

        var payload = new EditsRequest
        {
            //ID of the model to use. You can use the text-davinci-edit-001 or code-davinci-edit-001 model with this endpoint.
            Model = "text-davinci-edit-001",
            Input = "What day of the wek is it?",
            Instruction = "Fix the spelling mistakes",
        };

        var editResponse = await aiClient.GetEditsAsync(payload, CancellationToken.None);
        editResponse.Should().NotBeNull();

        string completion = editResponse!.Value.Choices[0].Text.Trim();
        completion.Should().NotBeNullOrWhiteSpace();
        completion.Should().Contain("What day of the week is it");
        output.WriteLine(completion);
    }


    [Fact]
    public async Task VerifyInitialAccessToEmbeddingsModelForClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IEmbeddingsAIClient>();

        var payload = new EmbeddingsRequest
        {
            Model = "text-embedding-ada-002",
            Input = "The food was delicious and the waiter...",
            User = "the user",
        };
        var serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string jsonContent = JsonSerializer.Serialize(payload, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        var embeddingsResponse = await aiClient.GetEmbeddingsAsync(payload, CancellationToken.None);
        embeddingsResponse.Should().NotBeNull();
        embeddingsResponse!.Value.Data.Count.Should().Be(1);
        var data = embeddingsResponse!.Value.Data[0];
        data.Embedding.Length.Should().Be(1536);
        output.WriteLine(data.Embedding.Length.ToString());
    }

    [Fact]
    public async Task VerifyFileuploadForClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();

        var payload = new UploadFileRequest
        {
            FullFilename = Path.Combine(path, "fine-tuning-data.jsonl"),
            Purpose = "fine-tune",
        };
        File.Exists(payload.FullFilename).Should().BeTrue();
        var response = await aiClient.UploadFilesAsync(payload, CancellationToken.None);
        response.Should().NotBeNull();

        response!.Value.Bytes.Should().Be(5514);
        response!.Value.Filename.Should().Be("fine-tuning-data.jsonl");
    }

    [Fact]
    public async Task VerifyListFilesForClient()
    {
        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        //https://platform.openai.com/docs/models/overview
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.FileData.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task VerifyDeleteAllFilesForClient()
    {
        await VerifyFileuploadForClient();

        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Should().NotBeNull();

        await Task.Delay(TimeSpan.FromSeconds(10));

        foreach (var fileData in response!.Value.FileData)
        {
            var responseDelete = await aiClient.DeleteFileAsync(fileData.Id, CancellationToken.None);
            responseDelete.Should().NotBeNull();
            responseDelete!.Value.Deleted.Should().BeTrue();
        }
        response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.FileData.Length.Should().Be(0);
    }


    [Fact]
    public async Task VerifyRetrieveFileInfoForClient()
    {
        await VerifyDeleteAllFilesForClient();
        await VerifyFileuploadForClient();
        await Task.Delay(TimeSpan.FromSeconds(10));

        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.FileData.Length.Should().Be(1);

        foreach (var fileData in response!.Value.FileData)
        {
            var responseFileInfo = await aiClient.RetrieveFileAsync(fileData.Id, CancellationToken.None);
            responseFileInfo.Should().NotBeNull();
            responseFileInfo!.Value.Bytes.Should().Be(5514);
            responseFileInfo!.Value.Filename.Should().Be("fine-tuning-data.jsonl");
        }

    }

    [Fact]
    public async Task VerifyRetrieveFileContentForClient()
    {
        await VerifyDeleteAllFilesForClient();
        await VerifyFileuploadForClient();
        await Task.Delay(TimeSpan.FromSeconds(10));

        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.FileData.Length.Should().Be(1);

        foreach (var fileData in response!.Value.FileData)
        {
            var responseFileContent = await aiClient.RetrieveFileContentAsync(fileData.Id, CancellationToken.None);
            responseFileContent.Should().NotBeNull();
            responseFileContent!.Value.Length.Should().Be(5514);
        }
    }


    [Fact]
    public async Task VerifyAccessToModels()
    {

        var aiClient = factory.Services.GetRequiredService<IModelsAIClient>();
        var response = await aiClient.GetModelsAsync(CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.ModelData.Length.Should().BeGreaterThan(0);

        foreach (var model in response!.Value.ModelData)
        {
            output.WriteLine(model.ModelID);
            var modelResponse = await aiClient.GetModelAsync(model.ModelID, CancellationToken.None);
            modelResponse.Should().NotBeNull();
            output.WriteLine(modelResponse.Value.ModelID);
        }
    }


    [Fact]
    public async Task VerifyCreateImage()
    {
        var aiClient = factory.Services.GetRequiredService<IImagesAIClient>();

        var payload = new ImageGenerationRequest
        {
            //Prompt = "A cute Filipino pair walking with umbrella, seen from the back while they walk against the sunset",
            Prompt = "A cute baby sea otter",
            NumberOfImagesToGenerate = 2,
            ImageSize = ImageSize.Size1024
        };

        var response = await aiClient.CreateImageAsync(payload, CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.Data.Length.Should().Be(2);

        foreach (var model in response!.Value.Data)
        {
            model.Url.Should().NotBeNullOrEmpty();
            model.Data.Should().BeNull();
            output.WriteLine(model.Url);
        }
    }
    [Fact]
    public async Task VerifyCreateImageWithBSonReturn()
    {
        var aiClient = factory.Services.GetRequiredService<IImagesAIClient>();

        var payload = new ImageGenerationRequest
        {
            Prompt = "A cute baby sea otter wearing a beret",
            NumberOfImagesToGenerate = 1,
            ImageSize = ImageSize.Size1024,
            ImageResponseFormat = ImageResponseFormat.B64Json
        };

        var response = await aiClient.CreateImageAsync(payload, CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.Data.Length.Should().Be(1);

        foreach (var model in response!.Value.Data)
        {
            model.Url.Should().BeNull();
            model.Data.Should().NotBeNull();
            output.WriteLine(model.Data);
        }
    }

    [Fact(Skip = "WIP")]
    public async Task VerifyEditImage()
    {
        var aiClient = factory.Services.GetRequiredService<IImagesAIClient>();
        string name = "img-rHHYCWOZmShIIIBNAco4l6WE.png";
        await using var imageStream = File.OpenRead(Path.Combine(path, name));

        var payload = new GenerateEditedImageRequest
        {
            Prompt = "A cute baby sea otter wearing a beret",
            Image = name,
            ImageStream = imageStream,
            //Mask = "@mask.png",
            NumberOfImagesToGenerate = 1,
            ImageSize = ImageSize.Size1024,
            ImageResponseFormat = ImageResponseFormat.Url
        };

        var response = await aiClient.CreateImageEditsAsync(payload, CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.Data.Length.Should().Be(2);

        foreach (var model in response!.Value.Data)
        {
            model.Url.Should().NotBeNullOrEmpty();
            model.Data.Should().BeNull();
            output.WriteLine(model.Url);
        }
    }

    [Fact]
    public async Task VerifyVariationsImage()
    {
        var aiClient = factory.Services.GetRequiredService<IImagesAIClient>();
        string name = "img-rHHYCWOZmShIIIBNAco4l6WE.png";
        await using var imageStream = File.OpenRead(Path.Combine(path, name));

        var payload = new GenerateVariationsOfImageRequest
        {
            Image = name,
            ImageStream = imageStream,
            NumberOfImagesToGenerate = 2,
            ImageSize = ImageSize.Size1024,
            ImageResponseFormat = ImageResponseFormat.Url
        };

        var response = await aiClient.CreateImageVariationsAsync(payload, CancellationToken.None);
        response.Should().NotBeNull();
        response!.Value.Data.Length.Should().Be(2);

        foreach (var model in response!.Value.Data)
        {
            model.Url.Should().NotBeNullOrEmpty();
            model.Data.Should().BeNull();
            output.WriteLine(model.Url);
        }
    }


}
