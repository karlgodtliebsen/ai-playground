using LLama;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Repositories;
using LLamaSharpApp.WebAPI.Domain.Services;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Chat Domain Service
/// </summary>
public class ChatService : IChatService
{
    private readonly ILlmaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;
    private readonly IOptionsService optionsService;
    private readonly ILogger<ChatService> logger;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    /// <summary>
    /// Constructor for Chat Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public ChatService(ILlmaModelFactory factory, IModelStateRepository modelStateRepository,
        IOptionsService optionsService, ILogger<ChatService> logger)
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
        var modelOptions = await optionsService.GetLlmaModelOptions(input.UserId, cancellationToken);
        var (chatSession, model) = factory.CreateChatSession<InstructExecutor>(modelOptions);

        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var outputs = chatSession.Chat(input.Text);
        var result = string.Join("", outputs);
        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        //model.Dispose();
        return result;
    }
}
