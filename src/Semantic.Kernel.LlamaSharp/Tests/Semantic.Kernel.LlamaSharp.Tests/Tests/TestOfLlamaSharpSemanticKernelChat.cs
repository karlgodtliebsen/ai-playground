using System.Security.Cryptography;

using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using LLama;
using LLama.Common;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.SemanticKernel.ChatCompletion;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.AI.ChatCompletion;

using SemanticKernel.Tests.Fixtures;

using Xunit.Abstractions;

namespace SemanticKernel.Tests;


[Collection("LlamaSharp SemanticKernel Collection")]
public class TestOfLlamaSharpSemanticKernelChat : IAsyncLifetime
{
    private readonly ILogger logger;

    private readonly ILoggerFactory loggerFactory;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly LlamaSharpSemanticKernelTestFixture fixture;
    private readonly LLamaModelOptions llamaModelOptions;
    private readonly InferenceOptions inferenceOptions;

    private const string CollectionName = "LlamaSharp-SemanticKernel-test-collection";

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }
    public TestOfLlamaSharpSemanticKernelChat(LlamaSharpSemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture
            .WithOutputLogSupport<TestFixtureBaseWithDocker>(output)
            //.WithQdrantSupport()
            .Build();
        var services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.inferenceOptions = services.GetRequiredService<IOptions<InferenceOptions>>().Value;
        this.llamaModelOptions = services.GetRequiredService<IOptions<LLamaModelOptions>>().Value;
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
    }

    /*
     llamasharp semantic kernel documentation
        ## IChatCompletion
        ```csharp
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        // LLamaSharpChatCompletion requires InteractiveExecutor, as it's the best fit for the given command.
        var ex = new InteractiveExecutor(context);
        var chatGPT = new LLamaSharpChatCompletion(ex);
        ```
    */


    [Fact]
    public async Task ExecuteTest()
    {
        logger.Information("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md");



        var modelPath = this.llamaModelOptions.ModelPath;
        var seed = 1337;

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            Seed = RandomNumberGenerator.GetInt32(int.MaxValue),
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var ex = new InteractiveExecutor(context);

        var chatGPT = new LLamaSharpChatCompletion(ex);

        var chatHistory = chatGPT.CreateNewChat("You are a librarian, expert about books");

        logger.Information("Chat content:");
        logger.Information("------------------------");

        chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");
        await MessageOutputAsync(chatHistory);

        // First bot assistant message
        var reply = await chatGPT.GenerateMessageAsync(chatHistory);
        chatHistory.AddAssistantMessage(reply);
        await MessageOutputAsync(chatHistory);

        // Second user message
        chatHistory.AddUserMessage("I love history and philosophy, I'd like to learn something new about Greece, any suggestion");
        await MessageOutputAsync(chatHistory);

        // Second bot assistant message
        reply = await chatGPT.GenerateMessageAsync(chatHistory);
        chatHistory.AddAssistantMessage(reply);
        await MessageOutputAsync(chatHistory);
    }

    /// <summary>
    /// Outputs the last message of the chat history
    /// </summary>
    private Task MessageOutputAsync(Microsoft.SemanticKernel.AI.ChatCompletion.ChatHistory chatHistory)
    {
        var message = chatHistory.Messages.Last();

        logger.Information($"{message.Role}: {message.Content}");
        logger.Information("------------------------");

        return Task.CompletedTask;
    }
}
