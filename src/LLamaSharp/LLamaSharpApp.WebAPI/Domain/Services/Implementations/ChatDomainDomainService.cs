using System.Runtime.CompilerServices;

using LLama;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Repositories;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Chat Domain Service
/// </summary>
public class ChatDomainDomainService : IChatDomainService
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
    public ChatDomainDomainService(ILlamaModelFactory factory,
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
        inferenceOptions.AntiPrompts = modelOptions.AntiPrompt;
        logger.Debug("Using Model Options: {@modelOptions}", modelOptions);
        logger.Debug("Using Inference Options: {@inferenceOptions}", inferenceOptions);

        var (chatSession, model) = factory.CreateChatSession<InstructExecutor>(modelOptions);
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var userInput = await CreateUserInput(input, modelOptions, cancellationToken);
        var outputs = chatSession.Chat(userInput, inferenceOptions, cancellationToken);
        var result = string.Join("", outputs);

        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        //model.Dispose();//TODO: check if this is needed
        return result;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> ChatStream(ChatMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        inferenceOptions.AntiPrompts = modelOptions.AntiPrompt;
        logger.Debug("Using Model Options: {@modelOptions}", modelOptions);
        logger.Debug("Using Inference Options: {@inferenceOptions}", inferenceOptions);

        var (chatSession, model) = factory.CreateChatSession<InstructExecutor>(modelOptions);

        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var userInput = await CreateUserInput(input, modelOptions, cancellationToken);
        var results = chatSession.ChatAsync(userInput, inferenceOptions, cancellationToken);
        await foreach (var result in results.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        //model.Dispose();//TODO: check if this is needed
    }

    private async Task<string> CreateUserInput(SimpleTextMessage input, LlamaModelOptions modelOptions, CancellationToken cancellationToken)
    {
        string userInput = input.Text;
        if (File.Exists(modelOptions.PromptFile))
        {
            logger.Debug("Prefixing Prompt file: {PromptFile} to User Input", modelOptions.PromptFile);
            userInput = await File.ReadAllTextAsync(modelOptions.PromptFile, cancellationToken) + input.Text;
        }
        logger.Debug("Using Input: {userInput}", userInput);
        return userInput;
    }
}
