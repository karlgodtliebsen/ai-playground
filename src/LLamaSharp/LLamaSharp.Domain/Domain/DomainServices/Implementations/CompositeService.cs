﻿using AI.Library.HttpUtils;

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

    ///// <summary>
    ///// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs" >LLama.Examples/a>
    ///// </summary>
    ///// <param name="input"></param>
    ///// <param name="cancellationToken"></param>
    ////public async Task<string> ChatUsingInstructionsSessionWithStrippedRoleNames(ExecutorInferMessage input, CancellationToken cancellationToken)
    ////{
    ////}

    /// <summary>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionWithRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    public async Task<OneOf<string, ErrorResponse>> ChatSessionWithInstructionsExecutorAndRoleName(ExecutorInferMessage input, CancellationToken cancellationToken)
    {
        try
        {
            var modelOptions = input.ModelOptions!;
            var inferenceOptions = input.InferenceOptions!;
            inferenceOptions.AntiPrompts = Array.Empty<string>(); //input.AntiPrompt!;
            //inferenceOptions.AntiPrompts = new List<string> { "User:" }!;

            var userInput = CreateUserInput(input);

            var outputs = executor.ChatUsingInteractiveExecutor(inferenceOptions, modelOptions, userInput);

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



    //public async Task<string> GetEmbeddings(ExecutorInferMessage input, CancellationToken cancellationToken)
    //{
    //}

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


    private void AlignModelParameters(ExecutorInferMessage input, InferenceOptions inferenceOptions, LlamaModelOptions modelOptions)
    {
        if (input.UseDefaultAntiPrompt && input.AntiPrompt is not null)
        {
            inferenceOptions.AntiPrompts = input.AntiPrompt;
        }

        if (input.ModelOptions is not null && input.AntiPrompt is not null)
        {
            inferenceOptions.AntiPrompts = input.AntiPrompt!;
        }
    }

    private string CreateUserInput(ExecutorInferMessage input)
    {
        string userInput = input.Text;

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
