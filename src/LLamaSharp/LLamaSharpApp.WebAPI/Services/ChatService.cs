using LLama.OldVersion;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Models;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Services;

public interface IChatService
{
    string Send(SendMessageInput input);
}

public class ChatService : IChatService
{
    private readonly ChatSession<LLamaModel> _session;
    private readonly LlmaOptions options;

    public ChatService(IOptions<LLamaModel> model, IOptions<LlmaOptions> options)
    {
        this.options = options.Value;
        _session = new ChatSession<LLamaModel>(model.Value)
            .WithPromptFile(this.options.PromptFile)
            .WithAntiprompt(this.options.AntiPrompt);
    }

    public string Send(SendMessageInput input)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(input.Text);

        Console.ForegroundColor = ConsoleColor.White;
        var outputs = _session.Chat(input.Text);
        var result = "";
        foreach (var output in outputs)
        {
            Console.Write(output);
            result += output;
        }

        return result;
    }
}
