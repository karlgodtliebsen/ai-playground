using LLama;

using LLamaSharpApp.WebAPI.Models;


namespace LLamaSharpApp.WebAPI.Services;

public class ChatService : IChatService
{
    private readonly ILlmaModelFactory factory;
    private readonly IStateHandler stateHandler;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    public ChatService(ILlmaModelFactory factory, IStateHandler stateHandler)
    {
        this.factory = factory;
        this.stateHandler = stateHandler;
    }

    public string Send(ChatMessage input)
    {
        var fileName = "./chat-savedstate.st";   //for now, we use local file system to store the state
                                                 //TODO: consider security etc. must be improved

        var (chatSession, model) = factory.CreateChatSession<InstructExecutor>();//TODO: handle user specific parameters to override the default ones
        stateHandler.LoadState(model, () => input.UsePersistedModelState ? fileName : null);
        var outputs = chatSession.Chat(input.Text);
        stateHandler.SaveState(model, () => input.UsePersistedModelState ? fileName : null);
        var result = string.Join("", outputs);

        //model.Dispose();

        return string.Join("", result);
    }
}
