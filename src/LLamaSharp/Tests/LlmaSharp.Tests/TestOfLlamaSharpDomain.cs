using System.Text;

using LLama;
using LLama.Common;

using LlamaSharp.Tests.Fixtures;

using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Services;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Xunit.Abstractions;

namespace LlamaSharp.Tests;

public sealed class TestOfLlamaSharpDomain : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly ITestOutputHelper output;
    private readonly IntegrationTestWebApplicationFactory factory;
    private readonly ILogger logger;

    public TestOfLlamaSharpDomain(IntegrationTestWebApplicationFactory factory, ITestOutputHelper output)
    {
        this.output = output;
        this.factory = factory.WithOutputLogSupport(output).Build<IntegrationTestWebApplicationFactory>();
        this.logger = factory.Logger;
    }
    public void Dispose()
    {
        Log.CloseAndFlush();
    }

    const string Prompt = @"
    Transcript of a dialog, where the User interacts with an Assistant named Bob.
    Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.
    User: Hello, Bob.
    Bob: Hello. How may I help you today?
    User: Please tell me the largest city in the World
    Bob: Sure. The largest city in the World is Tokyo, Japan.
    User:
";
    private static readonly string FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

    //LLamaSharp.Backend.Cuda1 1
    //LLamaSharp.Backend.Cuda12
    //LLamaSharp.Backend.Cpu

    [Fact]
    public async Task VerifyThatLLamaCanExecuteInteractiveExecutor()
    {
        string userId = factory.UserId;
        var sessionStateService = factory.Services.GetRequiredService<IContextStateRepository>();
        var optionsService = factory.Services.GetRequiredService<IOptionsService>();
        var modelOptions = optionsService.GetDefaultLlamaModelOptions();
        //sessionStateService.RemoveAllState(userId);

        var parameters = new ModelParams(modelOptions.ModelPath!)
        {
            ContextSize = 1024,
            Seed = 1337,
            GpuLayerCount = 5
        };
        using var model = LLamaWeights.LoadFromFile(parameters);

        // Initialize a chat session
        using var context = model.CreateContext(parameters);
        var ex = new InteractiveExecutor(context);

        var session = new ChatSession(ex);
        sessionStateService.LoadSession(session, userId, true);

        var prompt = Prompt;
        await RunPrompt(session, prompt);

        prompt = "What is the largest city in North America?";
        await RunPrompt(session, prompt);
        prompt = "What is the largest city in South America?";
        await RunPrompt(session, prompt);
        prompt = "What is the largest city in Asia?";
        await RunPrompt(session, prompt);

        prompt = "No thank you";
        await RunPrompt(session, prompt);

        sessionStateService.SaveSession(session, userId, true);
    }

    [Fact]
    public async Task VerifyThatLLamaExecuteInteractiveExecutorCanHandleSession()
    {
        string userId = Ulid.NewUlid().ToString();
        var sessionStateService = factory.Services.GetRequiredService<IContextStateRepository>();
        var optionsService = factory.Services.GetRequiredService<IOptionsService>();
        var modelOptions = optionsService.GetDefaultLlamaModelOptions();
        sessionStateService.RemoveAllState(userId);

        var parameters = new ModelParams(modelOptions.ModelPath!)
        {
            ContextSize = 1024,
            Seed = 1337,
            GpuLayerCount = 5
        };
        using var model = LLamaWeights.LoadFromFile(parameters);

        // Initialize a chat session
        using var context = model.CreateContext(parameters);
        var interactiveExecutor = new InteractiveExecutor(context);

        var session = new ChatSession(interactiveExecutor);

        var prompt = Prompt;
        await RunPrompt(session, prompt);
        sessionStateService.SaveSession(session, userId, true);

        session = new ChatSession(interactiveExecutor);
        sessionStateService.LoadSession(session, userId, true);
        await RunPrompt(session, prompt);
        sessionStateService.SaveSession(session, userId, true);

        //sessionStateService.RemoveAllState(userId);
    }

    private async Task RunPrompt(ChatSession session, string prompt)
    {
        var textBuilder = new StringBuilder();
        logger.Debug(prompt);

        var texts = session.ChatAsync(prompt, new InferenceParams()
        {
            Temperature = 0.6f,
            AntiPrompts = new List<string>
            {
                "User:"
            }
        });
        await foreach (var text in texts)
        {
            textBuilder.Append(text);
        }
        logger.Debug(textBuilder.ToString());
    }




}
