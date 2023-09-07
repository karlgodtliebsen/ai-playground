using System.Runtime.CompilerServices;

using LLama;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Repositories;

namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <summary>
/// Chat Domain Service
/// </summary>
public class ChatService : IChatService
{
    private readonly ILLamaFactory factory;
    private readonly IContextStateRepository contextStateRepository;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    /// <summary>
    /// Constructor for Chat Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="contextStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public ChatService(ILLamaFactory factory, IContextStateRepository contextStateRepository, IOptionsService optionsService, ILogger logger)
    {
        this.factory = factory;
        this.contextStateRepository = contextStateRepository;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// Executes the chat
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Chat(ChatMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        AlignModelParameters(input, inferenceOptions, modelOptions);
        var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);
        contextStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var userInput = CreateUserInput(input);
        var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);
        var result = string.Join("", outputs);

        contextStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        model.Dispose();        //TODO: check if this is needed - ResettableLLamaModel
        return result;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> ChatStream(ChatMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        AlignModelParameters(input, inferenceOptions, modelOptions);
        var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);    //InstructExecutor
        contextStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var userInput = CreateUserInput(input);
        var results = chatSession.ChatAsync(userInput, inferenceOptions, cancellationToken);
        await foreach (var result in results.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        model.Dispose();//TODO: check if this is needed - ResettableLLamaModel
    }


    private void AlignModelParameters(ChatMessage input, InferenceOptions inferenceOptions, LLamaModelOptions modelOptions)
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

    private string CreateUserInput(ChatMessage input)
    {
        string userInput = input.Text;

        if (input is { UseDefaultPrompt: false, Prompt: not null })
        {
            //var prompt = input.Prompt.Trim() + userInput;

            var prompt = $""""
                {input.Prompt.Trim()}\n
                """{userInput}"""
                """";

            logger.Debug("Prefixing User providedModel Options Prompt: {Prompt} to User Input {userInput}", prompt, userInput);
            return prompt;
        }

        if (input.UseDefaultPrompt && input.Prompt is not null)
        {
            //var prompt = $""""
            //    {input.Prompt.Trim()}\n
            //    """{userInput}"""
            //    """";
            var prompt = input.Prompt.Trim() + userInput;
            logger.Debug("Prefixing System Default Prompt template: {DefaultPrompt} to User Input {userInput}", prompt, userInput);
            return prompt;
        }

        logger.Debug("Using Input: {userInput}", userInput);
        return userInput;
    }
}
