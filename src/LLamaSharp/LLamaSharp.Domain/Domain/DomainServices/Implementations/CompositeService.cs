using AI.Library.HttpUtils;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Services;

using OneOf;

namespace LLamaSharp.Domain.Domain.DomainServices.Implementations;

/// <summary>
/// CompositeDomain Service that handles all the different composite chat actions
/// Created to align with the LLama.Examples
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/" >LLama.Examples</a>
/// </summary>
public class CompositeService : ICompositeService
{
    private readonly IInteractiveExecutorService interactiveExecutor;
    private readonly ILLamaFactory factory;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for Composite Service
    /// </summary>
    /// <param name="interactiveExecutor"></param>
    /// <param name="factory"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public CompositeService(IInteractiveExecutorService interactiveExecutor, ILLamaFactory factory, IOptionsService optionsService, ILogger logger)
    {
        this.interactiveExecutor = interactiveExecutor;
        this.factory = factory;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    public async Task<OneOf<string, ErrorResponse>> ChatSessionWithInstructionsExecutorAndNoRoleNames(ExecutorInferMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = input.ModelOptions!;
        var inferenceOptions = input.InferenceOptions!;
        inferenceOptions.AntiPrompts = Array.Empty<string>();
        var executionOptions = new KeywordTextOutputStreamTransform(input.Keywords, input.RedundancyLength, input.RemoveAllMatchedTokens);
        try
        {
            var userInput = CreateUserInput(input);
            var outputs = interactiveExecutor.ChatUsingInteractiveExecutorWithTransformation(inferenceOptions, modelOptions, executionOptions, userInput);
            var result = string.Join("", outputs);
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in ChatSessionWithInstructionsExecutorAndStrippedRoleName");
            return new ErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionWithRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    public async Task<OneOf<string, ErrorResponse>> ChatSessionWithInstructionsExecutorAndRoleNames(ExecutorInferMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = input.ModelOptions!;
        var inferenceOptions = input.InferenceOptions!;
        inferenceOptions.AntiPrompts = Array.Empty<string>();
        try
        {
            var userInput = CreateUserInput(input);
            var outputs = interactiveExecutor.ChatUsingInteractiveExecutor(inferenceOptions, modelOptions, userInput);
            var result = string.Join("", outputs);
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in ChatSessionWithInstructionsExecutorAndRoleName");
            return new ErrorResponse(ex.Message);
        }
        //model.Dispose();
    }

    //https://replicate.com/blog/how-to-prompt-llama

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    //public async Task<string> GetEmbeddings(ExecutorInferMessage input, CancellationToken cancellationToken)


    //SaveAndLoadSession
    //InstructModeExecute
    //InteractiveModeExecute
    //LoadAndSaveState
    //QuantizeModel
    //StatelessModeExecute

    //inferenceOptions.AntiPrompts = modelOptions.AntiPrompt!;
    //inferenceOptions.AntiPrompts = new List<string> { "User:" }!;

    //logger.Debug("Using Model Options: {@modelOptions}", modelOptions);
    //logger.Debug("Using Inference Options: {@inferenceOptions}", inferenceOptions);
    //AlignModelParameters(input, inferenceOptions, modelOptions);
    //var prompt = File.ReadAllText("Assets/chat-with-bob.txt").Trim();
    //InteractiveExecutor ex = new(new LLamaModel(new ModelParams(modelPath, contextSize: 1024, seed: 1337, gpuLayerCount: 5)));
    //ChatSession session = new ChatSession(ex); // The only change is to remove the transform for the output text stream.
    //var promptTemplate = await optionsService.GetSpecifiedSystemPromptTemplates("instruction", "1", cancellationToken);

    //var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions,
    //    (session =>
    //        session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
    //        new string[] { "User:", "Bob:" },
    //        redundancyLength: 8))));

    ////modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
    //var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);
    //modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);


    private void AlignModelParameters(ExecutorInferMessage input, InferenceOptions inferenceOptions, LLamaModelOptions modelOptions)
    {
        if (input.UseDefaultAntiPrompt && input.AntiPrompts is not null)
        {
            inferenceOptions.AntiPrompts = input.AntiPrompts;
        }

        if (input.ModelOptions is not null && input.AntiPrompts is not null)
        {
            inferenceOptions.AntiPrompts = input.AntiPrompts!;
        }
    }

    private string CreateUserInput(ExecutorInferMessage input)
    {
        //throw new NotImplementedException("Must be changed");

        string userInput = input!.Text;

        if (input is { UseDefaultPrompt: false, Prompt: not null })
        {
            var prompt = $""""
                {input.Prompt.Trim()}\n
                """{userInput}"""
                """";

            logger.Debug("Prefixing User providedModel Options Prompt: {Prompt} to User Input {userInput}", prompt, userInput);
            return prompt;
        }

        if (input is { UseDefaultPrompt: true, Prompt: not null })
        {
            var prompt = input.Prompt.Trim() + userInput;
            logger.Debug("Prefixing System Default Prompt template: {DefaultPrompt} to User Input {userInput}", prompt, userInput);
            return prompt;
        }

        logger.Debug("Using Input: {userInput}", userInput);
        return userInput;
    }
}
