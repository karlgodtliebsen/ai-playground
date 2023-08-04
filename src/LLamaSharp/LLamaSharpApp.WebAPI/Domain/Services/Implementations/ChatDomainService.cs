using System.Runtime.CompilerServices;

using LLama;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Repositories;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Chat Domain Service
/// </summary>
public class ChatDomainService : IChatDomainService
{
    private readonly ILlamaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    /// <summary>
    /// Constructor for Chat Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public ChatDomainService(ILlamaModelFactory factory,
        IModelStateRepository modelStateRepository, IOptionsService optionsService, ILogger logger)
    {
        this.factory = factory;
        this.modelStateRepository = modelStateRepository;
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
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var userInput = CreateUserInput(input, modelOptions);
        var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);
        var result = string.Join("", outputs);

        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        model.Dispose();        //TODO: check if this is needed - ResettableLLamaModel
        return result;
    }


    public async Task<string> ChatX(ChatMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        inferenceOptions.AntiPrompts = modelOptions.AntiPrompt;
        AlignModelParameters(input, inferenceOptions, modelOptions);

        //https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs

        modelOptions.ContextSize = 1024;
        modelOptions.Seed = 1337;
        //modelOptions.GpuLayerCount = 5;

        //logger.Debug("Using Model Options: {@modelOptions}", modelOptions);
        //logger.Debug("Using Inference Options: {@inferenceOptions}", inferenceOptions);

        var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions,
            (session => session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(new string[] { "User:", "Bob:" }, redundancyLength: 8))));


        //modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);

        var userInput = CreateUserInput(input, modelOptions);
        var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);
        var result = string.Join("", outputs);

        //modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);

        model.Dispose();
        return result;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> ChatStream(ChatMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        AlignModelParameters(input, inferenceOptions, modelOptions);
        var (chatSession, model) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);    //InstructExecutor
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var userInput = CreateUserInput(input, modelOptions);
        var results = chatSession.ChatAsync(userInput, inferenceOptions, cancellationToken);
        await foreach (var result in results.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        model.Dispose();//TODO: check if this is needed - ResettableLLamaModel
    }


    private void AlignModelParameters(ChatMessage input, InferenceOptions inferenceOptions, LlamaModelOptions modelOptions)
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

    private string CreateUserInput(ChatMessage input, LlamaModelOptions modelOptions)
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
