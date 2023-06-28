using LLama;

using LLamaSharpApp.WebAPI.Models;


namespace LLamaSharpApp.WebAPI.Services;

public class ChatService : IChatService
{
    private readonly ILlmaModelFactory factory;

    //TODO: consider https://github.com/SciSharp/LLamaSharp/blob/master/docs/ChatSession/save-load-session.md
    //TODO: use ChatHistory 

    public ChatService(ILlmaModelFactory factory)
    {
        this.factory = factory;
    }

    public string Send(SendMessage input)
    {
        //TODO: how to dispose LLamaModel
        //TODO: how to use the difference  between Interactive and Instruct Executor?
        var chatSession = factory.CreateChatSession<InstructExecutor>();
        var outputs = chatSession.Chat(input.Text);
        return string.Join("", outputs);
    }
}
