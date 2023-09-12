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
    /// <param name="chatMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Chat(ChatMessage chatMessage, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(chatMessage.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(chatMessage.UserId, cancellationToken);

        AlignModelParameters(chatMessage, inferenceOptions, modelOptions);

        var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);
        contextStateRepository.LoadState(model, chatMessage.UserId, chatMessage.UsePersistedModelState);

        var userInput = BuildUserInput(chatMessage);
        var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);
        var result = string.Join("", outputs);

        contextStateRepository.SaveState(model, chatMessage.UserId, chatMessage.UsePersistedModelState);
        model.Dispose();        //TODO: check if this is needed - ResettableLLamaModel
        return result;
    }
    //https://replicate.com/blog/how-to-prompt-llama

    /// <inheritdoc />
    public async IAsyncEnumerable<string> ChatStream(ChatMessage chatMessage, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(chatMessage.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(chatMessage.UserId, cancellationToken);

        AlignModelParameters(chatMessage, inferenceOptions, modelOptions);

        var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);    //InstructExecutor
        contextStateRepository.LoadState(model, chatMessage.UserId, chatMessage.UsePersistedModelState);

        var userInput = BuildUserInput(chatMessage);
        var results = chatSession.ChatAsync(userInput, inferenceOptions, cancellationToken);
        await foreach (var result in results.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        model.Dispose();//TODO: check if this is needed - ResettableLLamaModel
    }


    private void AlignModelParameters(ChatMessage chatMessage, InferenceOptions inferenceOptions, LLamaModelOptions modelOptions)
    {
        if (chatMessage.UseDefaultAntiPrompt && chatMessage.AntiPrompts is not null)
        {
            inferenceOptions.AntiPrompts = chatMessage.AntiPrompts;
        }

        if (chatMessage.ModelOptions is not null && chatMessage.AntiPrompts is not null)
        {
            inferenceOptions.AntiPrompts = chatMessage.AntiPrompts!;
        }

        if (chatMessage.InferenceOptions is not null && Math.Abs(chatMessage.InferenceOptions.Temperature - 1.0f) > float.Epsilon)
        {
            inferenceOptions.Temperature = chatMessage.InferenceOptions.Temperature;
        }

    }
    /// <summary>
    /// <a href="https://replicate.com/blog/how-to-prompt-llama">Llama 2 Prompt</a>
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string BuildUserInput(ChatMessage input)
    {
        string userInput = input.Text;

        if (input is { UseDefaultPrompt: false, Prompt: not null, SystemPrompt: not null })
        {
            var prompt = BuildPromptBasedUserInput(input.Prompt, input.SystemPrompt);
            return prompt;
        }

        if (input is { UseDefaultPrompt: false, Prompt: null, Text: not null })
        {
            var prompt = userInput + $""" "prompt":"{input.Text}" """.Trim();
            logger.Debug("Using prompt: {prompt}", prompt);
            return prompt;
        }

        if (input is { UseDefaultPrompt: false, Prompt: not null })
        {
            var prompt = $"""" "prompt":"{input.Prompt.Trim()}"\n """{userInput}""" """".Trim();
            logger.Debug("Prefixing User providedModel Options Prompt: {@input} to User Input {Prompt}", prompt, input);
            return prompt;
        }

        if (input.UseDefaultPrompt && input.Prompt is not null)
        {
            var prompt = input.Prompt.Trim() + userInput;
            if (!string.IsNullOrWhiteSpace(input.SystemPrompt))
            {
                prompt += $""""""\n"systemprompt":"{input.SystemPrompt}" """""";
            }
            logger.Debug("Prefixing System Default Prompt template: {@input} to User Input {Prompt}", prompt, input);
            return prompt;
        }

        logger.Debug("Using Input: {userInput}", userInput);
        return userInput;
    }

    private string BuildPromptBasedUserInput(string prompt, string systemPrompt)
    {
        var modelInput = $"""" "prompt":"{prompt.Trim()}"\n"systemprompt":"{systemPrompt.Trim()}" """".Trim();
        logger.Debug("Using Prompt: {Prompt} and SystemPrompt: {systemPrompt} to generate input: {modelInput}", prompt, systemPrompt, modelInput);
        return modelInput;
    }

}
