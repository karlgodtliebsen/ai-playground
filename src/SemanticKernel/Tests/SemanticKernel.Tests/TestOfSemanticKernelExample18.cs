using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.ImageGeneration;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample18
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly OpenAIOptions openAIOptions;
    private readonly SemanticKernelTestFixtureBase fixture;

    public TestOfSemanticKernelExample18(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Output = output;
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.openAIOptions = fixture.OpenAIOptions;
    }

    [Fact]
    public async Task UseDallEQdrantMemoryCollectionc_Example18()
    {
        logger.Information("======== OpenAI Dall-E 2 Image Generation ========");

        var chatCompletionModel = "gpt-3.5-turbo";
        //const int openAiVectorSize = 1536;
        //bool recreateCollection = true;
        //bool storeOnDisk = false;
        //var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        //var memoryStorage = await factory.Create(collectionName, openAiVectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        IKernel kernel = new KernelBuilder()
            .WithLogger(fixture.MsLogger)
            .WithOpenAIImageGenerationService(openAIOptions.ApiKey)
            .WithOpenAIChatCompletionService(chatCompletionModel, openAIOptions.ApiKey)
            //.WithMemoryStorage(memoryStorage)
            .Build();

        IImageGeneration dallE = kernel.GetService<IImageGeneration>();

        var imageDescription = "A cute baby elephant";
        var image = await dallE.GenerateImageAsync(imageDescription, 256, 256);

        logger.Information(imageDescription);
        logger.Information("Image URL: " + image);

        logger.Information("======== Chat with images ========");

        IChatCompletion chatGPT = kernel.GetService<IChatCompletion>();
        var chatHistory = chatGPT.CreateNewChat(
            "You're chatting with a user. Instead of replying directly to the user" +
            " provide the description of an image that expresses what you want to say." +
            " The user won't see your message, they will see only the image. The system " +
            " generates an image using your description, so it's important you describe the image with details.");

        var msg = "Hi, I'm from Tokyo, where are you from?";
        chatHistory.AddUserMessage(msg);
        logger.Information("User: " + msg);

        string reply = await chatGPT.GenerateMessageAsync(chatHistory);
        chatHistory.AddAssistantMessage(reply);
        image = await dallE.GenerateImageAsync(reply, 256, 256);
        logger.Information("Bot: " + image);
        logger.Information("Img description: " + reply);

        msg = "Oh, wow. Not sure where that is, could you provide more details?";
        chatHistory.AddUserMessage(msg);
        logger.Information("User: " + msg);

        reply = await chatGPT.GenerateMessageAsync(chatHistory);
        chatHistory.AddAssistantMessage(reply);
        image = await dallE.GenerateImageAsync(reply, 256, 256);
        logger.Information("Bot: " + image);
        logger.Information("Img description: " + reply);

        /* Output:

        User: Hi, I'm from Tokyo, where are you from?
        Bot: https://oaidalleapiprodscus.blob.core.windows.net/private/...
        Img description: [An image of a globe with a pin dropped on a location in the middle of the ocean]

        User: Oh, wow. Not sure where that is, could you provide more details?
        Bot: https://oaidalleapiprodscus.blob.core.windows.net/private/...
        Img description: [An image of a map zooming in on the pin location, revealing a small island with a palm tree on it]
        */
    }


}
