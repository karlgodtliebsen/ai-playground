using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Services;

namespace LLamaSharp.Domain.Domain.DomainServices.Implementations;

/// <summary>
/// CompositeDomain Service that handles all the different composite chat actions
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs" >llamasharp samples</a>
/// </summary>
public class CompositeService : ICompositeService
{
    private readonly IExecutorService executor;
    private readonly ILlamaModelFactory factory;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    /// <summary>
    /// Constructor for Composite Service
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="factory"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public CompositeService(IExecutorService executor, ILlamaModelFactory factory, IOptionsService optionsService, ILogger logger)
    {
        this.executor = executor;
        this.factory = factory;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs" >llamasharp samples</a>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    public async Task<string> ChatUsingInstructionsSessionWithRoleName(ExecutorInferMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);

        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);

        //        inferenceOptions.AntiPrompts = modelOptions.AntiPrompt!;
        inferenceOptions.AntiPrompts = new List<string> { "User:" }!;
        inferenceOptions.Temperature = 0.6f;

        //https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs

        modelOptions.ContextSize = 1024;
        modelOptions.Seed = 1337;
        //modelOptions.GpuLayerCount = 5;

        //logger.Debug("Using Model Options: {@modelOptions}", modelOptions);
        //logger.Debug("Using Inference Options: {@inferenceOptions}", inferenceOptions);

        //AlignModelParameters(input, inferenceOptions, modelOptions);


        var promptTemplate = await optionsService.GetSpecifiedSystemPromptTemplates("instruction", "1", cancellationToken);

        //fill the template with the user input
        var userInput = CreateUserInput(input, promptTemplate, modelOptions);

        var outputs = executor.ChatUsingInteractiveExecutor(inferenceOptions, modelOptions, userInput);


        //var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions,
        //    (session =>
        //        session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
        //        new string[] { "User:", "Bob:" },
        //        redundancyLength: 8))));


        ////modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);

        //var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);

        //modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        var result = string.Join("", outputs);

        //model.Dispose();
        return result;
    }

    private void AlignModelParameters(ExecutorInferMessage input, InferenceOptions inferenceOptions, LlamaModelOptions modelOptions)
    {
        if (input.UseDefaultAntiPrompt && modelOptions.AntiPrompt is not null)
        {
            inferenceOptions.AntiPrompts = modelOptions.AntiPrompt;
        }

        if (input.ModelOptions is not null && input.ModelOptions.AntiPrompt is not null)
        {
            inferenceOptions.AntiPrompts = input.ModelOptions.AntiPrompt!;
        }
    }
    private string CreateUserInput(ExecutorInferMessage input, string promptTemplate, LlamaModelOptions modelOptions)
    {
        string userInput = input.Text;

        if (input is { UseDefaultPrompt: false, ModelOptions.Prompt: not null })
        {
            var prompt = input.ModelOptions.Prompt.Trim() + userInput;
            logger.Debug("Prefixing User providedModel Options Prompt: {Prompt} to User Input {userInput}", prompt, userInput);
            return prompt;
        }

        if (input.UseDefaultPrompt && modelOptions.Prompt is not null)
        {
            var prompt = modelOptions.Prompt.Trim() + userInput;
            logger.Debug("Prefixing System Default Prompt template: {DefaultPrompt} to User Input {userInput}", prompt, userInput);
            return prompt;
        }

        logger.Debug("Using Input: {userInput}", userInput);
        return userInput;
    }
}
