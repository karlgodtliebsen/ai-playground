using System.Text;

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
            var outputs = interactiveExecutor.ChatUsingInteractiveExecutorWithTransformation(inferenceOptions, modelOptions, executionOptions, userInput, cancellationToken);
            var result = new StringBuilder();
            await foreach (string o in outputs.WithCancellation(cancellationToken))
            {
                if (!string.IsNullOrEmpty(o))
                {
                    result.Append(o);
                }
            }
            logger.Information("ChatSessionWithInstructionsExecutorAndNoRoleNames");
            logger.Information(result.ToString());
            return result.ToString();
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
            var outputs = interactiveExecutor.ChatUsingInteractiveExecutor(inferenceOptions, modelOptions, userInput, cancellationToken);
            var result = new StringBuilder();
            await foreach (string o in outputs.WithCancellation(cancellationToken))
            {
                if (!string.IsNullOrEmpty(o))
                {
                    result.Append(o);
                }
            }
            logger.Information("ChatSessionWithInstructionsExecutorAndRoleNames");
            logger.Information(result.ToString());
            return result.ToString();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in ChatSessionWithInstructionsExecutorAndRoleName");
            return new ErrorResponse(ex.Message);
        }
        //model.Dispose();
    }

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
