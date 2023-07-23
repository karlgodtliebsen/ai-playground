using AI.Test.Support;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.Chat;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Images;
using OpenAI.Client.OpenAI.Models.Requests;

using Xunit.Abstractions;

namespace OpenAI.Tests;


public class TestOfOpenAIClients
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly string path;
    private readonly IModelRequestFactory requestFactory;

    public TestOfOpenAIClients(ITestOutputHelper output)
    {
        this.output = output;
        this.factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddOpenAIConfiguration(configuration);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        factory.ConfigureLogging(output);
        logger = factory.Services.GetRequiredService<ILogger>();
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        requestFactory = factory.Services.GetRequiredService<IModelRequestFactory>();
    }


    [Fact]
    public async Task VerifyDownloadModelsClient()
    {

        var aiClient = factory.Services.GetRequiredService<IModelsAIClient>();
        var response = await aiClient.GetModelsAsync(CancellationToken.None);
        response.Switch(
            r =>
            {
                var models = r.ModelData;
                models.Length.Should().BeGreaterThan(0);
                foreach (var model in models)
                {
                    output.WriteLine(model.Id);
                }
            },
        error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyTextCompletionModelClient()
    {
        var aiClient = factory.Services.GetRequiredService<ICompletionAIClient>();

        //https://platform.openai.com/docs/models/overview
        //text-davinci-003
        //Can do any language task with better quality, longer output, and consistent instruction-following
        //than the curie, babbage, or ada models. Also supports some additional features such as inserting text.

        string deploymentName = "text-davinci-003"; //text-davinci-003 text-davinci-002
        string prompt = "Say this is a test";
        output.WriteLine($"Input: {prompt}");

        var payload = requestFactory.CreateRequest<CompletionRequest>(() =>
            new CompletionRequest
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
            });

        var response = await aiClient.GetCompletionsAsync(payload, CancellationToken.None);
        response.Switch(
            r =>
            {
                string completion = r.Choices[0].Text.Trim();
                completion.Should().NotBeNullOrWhiteSpace();
                completion.Should().Be("This is indeed a test");
                output.WriteLine(completion);
            },
            error => throw new AIException(error.Error)
        );

    }
    [Fact]
    public async Task VerifyCompletionModelClient()
    {
        var aiClient = factory.Services.GetRequiredService<ICompletionAIClient>();

        //https://platform.openai.com/docs/models/overview
        //text-davinci-003
        //Can do any language task with better quality, longer output, and consistent instruction-following
        //than the curie, babbage, or ada models. Also supports some additional features such as inserting text.

        string deploymentName = "text-davinci-003"; //text-davinci-003 text-davinci-002

        var payload = requestFactory.CreateRequest<CompletionRequest>(() =>
            new CompletionRequest
            {
                Model = deploymentName,
                UsePrompts = new[]
                    {
                        "Today is Monday, tomorrow is",
                        "10 11 12 13 14"
                    },
                Temperature = 0,
                MaxTokens = 300,
                TopP = 1.0f,
                NumChoicesPerPrompt = 1,
                Stream = false,
                Logprobs = null,
                //Echo = true,
                //Stop = "\n"
            });

        var response = await aiClient.GetCompletionsAsync(payload, CancellationToken.None);
        response.Switch(
            completions =>
            {
                completions.Choices.Count.Should().Be(1);
                string completion = completions.Choices[0].Text.Trim();
                completion.Should().Contain("Tuesday");
                output.WriteLine(completion);
            },
            error => throw new AIException(error.Error)
            );
    }

    [Fact]
    public async Task VerifyChatCompletionModelClient()
    {
        //https://platform.openai.com/docs/models/overview
        //https://platform.openai.com/docs/api-reference/chat/create

        var aiClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role = ChatMessageRole.System.AsOpenAIRole(), Content = "You are a helpful assistant.!" },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = "Hello!" }
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages
            });

        var response = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
        response.Should().NotBeNull();

        response.Switch(
            completions =>
            {
                completions.Choices.Count.Should().Be(1);
                string completion = completions.Choices.First().Message!.Content!.Trim();
                completion.Should().Contain("I assist you today?");
                output.WriteLine(completion);
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyChatCompletionModelUsingLongAnswerAndStreamClient()
    {
        //https://platform.openai.com/docs/models/overview
        //https://platform.openai.com/docs/api-reference/chat/create

        var aiClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role = ChatMessageRole.System.AsOpenAIRole(), Content = "You are a helpful assistant.!" },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = "Count to 100, with a comma between each number and no newlines. E.g., 1, 2, 3, ..." }
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages,
                Stream = true,
                MaxTokens = 200,
            });

        var response = await aiClient.GetChatCompletionsUsingStreamAsync(payload, CancellationToken.None);
        response.Switch(
            completions =>
            {
                completions.Data.Count.Should().Be(202);
                foreach (var v in completions.Data)
                {
                    foreach (var t in v.Choices)
                    {
                        if (t!.Delta!.Content != null)
                        {
                            string completion = t!.Delta!.Content;
                            output.WriteLine(completion);
                        }
                    }
                }
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyChatCompletionModelUsingLongAnswerAndGenericStreamClient()
    {
        //https://platform.openai.com/docs/models/overview
        //https://platform.openai.com/docs/api-reference/chat/create

        var aiClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role = ChatMessageRole.System.AsOpenAIRole(), Content = "You are a helpful assistant.!" },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = "Count to 100, with a comma between each number and no newlines. E.g., 1, 2, 3, ..." }
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages,
                Stream = true,
                MaxTokens = 200,
            });

        var response = await aiClient.GetChatCompletionsUsingStreamAsync(payload, CancellationToken.None);

        response.Switch(
            completions =>
            {
                completions.Data.Count.Should().Be(202);
                foreach (var v in completions.Data)
                {
                    foreach (var t in v.Choices)
                    {
                        if (t!.Delta!.Content != null)
                        {
                            string completion = t!.Delta!.Content;
                            output.WriteLine(completion);
                        }
                    }
                }
            },
            error => throw new AIException(error.Error)
        );
    }


    [Fact]
    public async Task VerifyChatCompletionModelUsingStreamClient()
    {
        //https://platform.openai.com/docs/models/overview
        //https://platform.openai.com/docs/api-reference/chat/create
        //https://github.com/openai/openai-cookbook/blob/main/examples/How_to_stream_completions.ipynb

        var aiClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role =ChatMessageRole.System.AsOpenAIRole(), Content = "You are a helpful assistant.!" },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = "Count to 100, with a comma between each number and no newlines. E.g., 1, 2, 3, ..." }
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages,
                Stream = true,
                MaxTokens = 200,
            });

        var responseCollection = aiClient.GetChatCompletionsStreamAsync(payload, CancellationToken.None);
        await foreach (var response in responseCollection)
        {
            response.Switch(
                completions =>
                {
                    foreach (var t in completions.Choices)
                    {
                        if (t!.Delta!.Content != null)
                        {
                            string completion = t!.Delta!.Content;
                            output.WriteLine(completion);
                        }
                    }
                },
                error => throw new AIException(error.Error)
            );
        }
    }


    [Fact]
    public async Task VerifyChatCompletionModel2Client()
    {
        //https://platform.openai.com/docs/models/overview
        //https://platform.openai.com/docs/api-reference/chat/create

        var aiClient = factory.Services.GetRequiredService<IChatCompletionAIClient>();

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role = ChatMessageRole.System.AsOpenAIRole(), Content = "You are a helpful assistant.!" },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = "What is the population in Denmark!" }
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages
            });

        var response = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);

        response.Switch(
            completions =>
            {
                completions.Choices.Count.Should().Be(1);
                string completion = completions.Choices.First().Message!.Content!.Trim();
                completion.Should().NotBeNullOrWhiteSpace();
                output.WriteLine(completion);
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyModerationModelClient()
    {
        //https://platform.openai.com/docs/models/overview
        //https://platform.openai.com/docs/api-reference/moderations/create

        var aiClient = factory.Services.GetRequiredService<IModerationAIClient>();

        string deploymentName = "text-moderation-latest";

        var payload = requestFactory.CreateRequest<ModerationRequest>(() =>
            new ModerationRequest
            {
                Model = deploymentName,
                Input = "This is a Test. Please apply negative moderation"// I do not want to send a bad sentence to the API
            });

        var response = await aiClient.GetModerationAsync(payload, CancellationToken.None);
        response.Switch(
            r =>
            {
                r.Results.Should().NotBeNull();//Shallow check
            },
            error => throw new AIException(error.Error)
        );
    }


    [Fact]
    public async Task VerifyEditModelClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IEditsAIClient>();

        var payload = requestFactory.CreateRequest<EditsRequest>(() =>
            new EditsRequest
            {
                //ID of the model to use. You can use the text-davinci-edit-001 or code-davinci-edit-001 model with this endpoint.
                Model = "text-davinci-edit-001",
                Input = "What day of the wek is it?",
                Instruction = "Fix the spelling mistakes",
            }
        );

        var response = await aiClient.GetEditsAsync(payload, CancellationToken.None);
        response.Switch(
            completions =>
            {
                completions.Should().NotBeNull();
                string completion = completions.Choices[0].Text.Trim();
                completion.Should().NotBeNullOrWhiteSpace();
                completion.Should().Contain("What day of the week is it");
                output.WriteLine(completion);
            }
            , error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyEmbeddingsModelClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IEmbeddingsAIClient>();

        var payload = requestFactory.CreateRequest<EmbeddingsRequest>(() =>
            new EmbeddingsRequest
            {
                Model = "text-embedding-ada-002",
                Input = "The food was delicious and the waiter...",
                User = "the user",
            });

        var response = await aiClient.GetEmbeddingsAsync(payload, CancellationToken.None);
        response.Switch(
            embeddings =>
            {
                embeddings.Data.Count.Should().Be(1);
                var data = embeddings.Data[0];
                data.Embedding.Length.Should().Be(1536);
                output.WriteLine(data.Embedding.Length.ToString());
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyFileUploadClient()
    {
        //https://platform.openai.com/docs/models/overview
        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();

        var payload =
            new UploadFileRequest
            {
                FullFilename = Path.Combine(path, "fine-tuning-data.jsonl"),
                Purpose = "fine-tune",
            };

        File.Exists(payload.FullFilename).Should().BeTrue();
        var response = await aiClient.UploadFilesAsync(payload, CancellationToken.None);
        response.Switch(
            fileData =>
            {
                fileData.Bytes.Should().Be(5514);
                fileData.Filename.Should().Be("fine-tuning-data.jsonl");
            },
            error => throw new Exception(error.Error)
        );
    }

    [Fact]
    public async Task VerifyListFilesClient()
    {
        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        //https://platform.openai.com/docs/models/overview
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Switch(
            fileData =>
            {
                fileData.FileData.Length.Should().BeGreaterThan(0);
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyDeleteAllFilesClient()
    {
        await VerifyFileUploadClient();

        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Switch(
            _ => { },
            error => throw new AIException(error.Error)
        );

        await Task.Delay(TimeSpan.FromSeconds(10));
        foreach (var fileData in response!.AsT0.FileData)
        {
            var responseDelete = await aiClient.DeleteFileAsync(fileData.Id, CancellationToken.None);
            responseDelete.Switch(
                fd =>
                {
                    fd.Deleted.Should().BeTrue();
                },
                error => throw new Exception(error.Error)
            );
        }
        response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Switch(
            fileData =>
            {
                fileData.FileData.Length.Should().Be(0);
            },
            error => throw new AIException(error.Error)
        );
    }


    [Fact]
    public async Task VerifyRetrieveFileInfoClient()
    {
        await VerifyDeleteAllFilesClient();
        await VerifyFileUploadClient();
        await Task.Delay(TimeSpan.FromSeconds(10));

        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Should().NotBeNull();
        response.Switch(
            fd =>
            {
                fd.FileData.Length.Should().Be(1);
            },
            error => throw new AIException(error.Error)
        );

        foreach (var fileData in response!.AsT0.FileData)
        {
            var responseFileInfo = await aiClient.RetrieveFileAsync(fileData.Id, CancellationToken.None);
            responseFileInfo.Should().NotBeNull();
            responseFileInfo.Switch(
                fd =>
                {
                    fd.Bytes.Should().Be(5514);
                    fd.Filename.Should().Be("fine-tuning-data.jsonl");
                },
                error => throw new AIException(error.Error)
            );
        }
    }

    [Fact]
    public async Task VerifyRetrieveFileContentClient()
    {
        await VerifyDeleteAllFilesClient();
        await VerifyFileUploadClient();
        await Task.Delay(TimeSpan.FromSeconds(10));

        var aiClient = factory.Services.GetRequiredService<IFilesAIClient>();
        var response = await aiClient.GetFilesAsync(CancellationToken.None);
        response.Switch(
            fd =>
            {
                fd.FileData.Length.Should().Be(1);
            },
            error => throw new AIException(error.Error)
        );

        foreach (var fileData in response!.AsT0.FileData)
        {
            var responseFileContent = await aiClient.RetrieveFileContentAsync(fileData.Id, CancellationToken.None);
            responseFileContent.Switch(
                r =>
                {
                    r.Length.Should().Be(5514);
                },
                error => throw new AIException(error.Error)
            );
        }
    }


    [Fact]
    public async Task VerifyCreateImageClient()
    {
        var aiClient = factory.Services.GetRequiredService<IImagesAIClient>();
        var payload =

            new ImageGenerationRequest
            {
                Prompt = "A cute baby sea otter",//A cyberpunk monkey hacker dreaming of a beautiful bunch of bananas, digital art
                NumberOfImagesToGenerate = 2,
                ImageSize = ImageSize.Size1024
            };

        var response = await aiClient.CreateImageAsync(payload, CancellationToken.None);
        response.Switch(
            r =>
            {
                r.Data.Length.Should().Be(2);
                foreach (var model in r.Data)
                {
                    model.Url.Should().NotBeNullOrEmpty();
                    model.Data.Should().BeNull();
                    output.WriteLine(model.Url);
                }
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact]
    public async Task VerifyCreateImageWithBSonReturnClient()
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
        response.Switch(
            r =>
            {
                r.Data.Length.Should().Be(1);
                foreach (var model in r.Data)
                {
                    model.Url.Should().BeNullOrEmpty();
                    model.Data.Should().NotBeNullOrEmpty();
                    output.WriteLine(model.Data);
                }
            },
            error => throw new AIException(error.Error)
        );
    }

    [Fact(Skip = "WIP")]
    public async Task VerifyEditImageClient()
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
        response.Switch(
            r =>
            {
                r.Data.Length.Should().Be(1);
                foreach (var model in r.Data)
                {
                    model.Url.Should().NotBeNullOrEmpty();
                    model.Data.Should().BeNull();
                    output.WriteLine(model.Url);
                }
            },
            error => throw new AIException(error.Error)
        );

    }

    [Fact]
    public async Task VerifyVariationsImageClient()
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
        response.Switch(
            r =>
            {
                r.Data.Length.Should().Be(2);
                foreach (var model in r.Data)
                {
                    model.Url.Should().NotBeNullOrEmpty();
                    model.Data.Should().BeNull();
                    output.WriteLine(model.Url);
                }
            },
            error => throw new AIException(error.Error)
        );

    }


}
