using LLama;

using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Repositories;

namespace LLamaSharpApp.WebAPI.Services.Implementations;

public class ChatService : IChatService
{
    private readonly ILlmaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    public ChatService(ILlmaModelFactory factory, IModelStateRepository modelStateRepository)
    {
        this.factory = factory;
        this.modelStateRepository = modelStateRepository;
    }

    public string Chat(ChatMessage input)
    {
        var fileName = "./chat-savedstate.st";   //for now, we use local file system to store the state
                                                 //TODO: consider security etc. must be improved

        var (chatSession, model) = factory.CreateChatSession<InstructExecutor>();//TODO: handle user specific parameters to override the default ones
        modelStateRepository.LoadState(model, () => input.UsePersistedModelState ? fileName : null);
        var outputs = chatSession.Chat(input.Text);
        modelStateRepository.SaveState(model, () => input.UsePersistedModelState ? fileName : null);
        var result = string.Join("", outputs);

        //model.Dispose();

        return string.Join("", result);
    }
}
